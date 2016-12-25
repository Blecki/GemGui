using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Linq;

namespace GemGui
{
	/// <summary>
	/// An individual element in the GUI.
	/// </summary>
    public class Widget
    {
        public Rectangle Rect;
        internal List<Widget> Children = new List<Widget>();
        public Widget Parent { get; private set; }
        public Root Root { get; internal set; }
        public WidgetProperties Properties { get; private set; }

        private Mesh CachedRenderMesh = null;
        
        public Widget() { }

        internal void Construct(Rectangle Rect, WidgetProperties Settings, Root Root)
        {
            this.Rect = Rect;
            this.Properties = Settings;
            if (this.Properties == null) throw new InvalidOperationException();
            this.Root = Root;
            this.OnConstruct();
        }

        internal virtual void OnConstruct() { }

        public Widget FindWidgetAt(int x, int y)
        {
            foreach (var child in Children)
            {
                var item = child.FindWidgetAt(x, y);
                if (item != null) return item;
            }

            if (Rect.Contains(x, y)) return this;
            return null;
        }

        public void Invalidate()
        {
            CachedRenderMesh = null;
            if (Parent != null) Parent.Invalidate();
        }

        public Widget AddChild(Widget child)
		{
            if (child.Root != this.Root) throw new InvalidOperationException("Can't add UIItem to different heirarchy");

			Children.Add(child);
			child.Parent = this;
            child.Properties.InheritFrom(this.Properties);
            Invalidate();

            return child;
		}
		
		public void RemoveChild(Widget child)	
        {
            Children.Remove(child);
            child.Parent = null;
            child.Properties.InheritFrom(null);
            Invalidate();
        }

        public void Close()
        {
            Root.DestroyWidget(this);       
        }

        public virtual Rectangle GetDrawableInterior()
        {
            var border = Properties.Border;
            if (border == null) return Rect;
            else
            {
                var tileSheet = Root.TileSheets[border];
                return Rect.Interior(tileSheet.TileWidth, tileSheet.TileHeight, tileSheet.TileWidth, tileSheet.TileHeight);
            }
        }

        protected virtual Mesh Redraw()
        {
            if (Properties.Transparent)
                return Mesh.EmptyMesh();

            var result = new List<Mesh>();

            var background = Properties.Background;
            if (background != null)
                result.Add(Mesh.CreateSpriteQuad()
                    .Transform(Matrix.CreateScale(Rect.Width, Rect.Height, 1.0f))
                    .Transform(Matrix.CreateTranslation(Rect.X, Rect.Y, 0.0f))
                    .Colorize(Properties.BackgroundColor)
                    .TransformTexture(Root.TileSheets[background.Sheet].TileMatrix(background.Tile)));

            var border = Properties.Border;
            if (border != null)
            {
                //Create a 'scale 9' background 
                result.Add(
                    Mesh.CreateScale9Background(Rect, Root.TileSheets[border])
                    .Colorize(Properties.BackgroundColor));
            }

            // Add text label
            var label = Properties.Text;
            if (label != null)
            {
                var stringMeshSize = new Rectangle();
                var font = Root.TileSheets["Font"];
                var textSize = Properties.TextSize;
                var stringMesh = Mesh.CreateStringMesh(
                    label,
                    font,
                    new Vector2(font.TileWidth * textSize, font.TileHeight * textSize),
                    out stringMeshSize)
                    .Colorize(Properties.TextColor);

                var drawableArea = GetDrawableInterior();

                var textDrawPos = Vector2.Zero;

                switch (Properties.TextHorizontalAlign)
                {
                    case WidgetProperties.HorizontalAlign.Left:
                        textDrawPos.X = drawableArea.X;
                        break;
                    case WidgetProperties.HorizontalAlign.Right:
                        textDrawPos.X = drawableArea.X + drawableArea.Width - stringMeshSize.Width;
                        break;
                    case WidgetProperties.HorizontalAlign.Center:
                        textDrawPos.X = drawableArea.X + ((drawableArea.Width - stringMeshSize.Width) / 2);
                        break;
                }

                switch (Properties.TextVerticalAlign)
                {
                    case WidgetProperties.VerticalAlign.Top:
                        textDrawPos.Y = drawableArea.Y;
                        break;
                    case WidgetProperties.VerticalAlign.Bottom:
                        textDrawPos.Y = drawableArea.Y + drawableArea.Height - stringMeshSize.Height;
                        break;
                    case WidgetProperties.VerticalAlign.Center:
                        textDrawPos.Y = drawableArea.Y + ((drawableArea.Height - stringMeshSize.Height) / 2);
                        break;
                }

                stringMesh.Transform(Matrix.CreateTranslation(textDrawPos.X, textDrawPos.Y, 0.0f));
                result.Add(stringMesh);
            }

            return Mesh.Merge(result.ToArray());
        }

        public Mesh PrepareRenderMesh()
        {
            if (CachedRenderMesh == null)
            {
                var r = new Mesh[1 + Children.Count];
                r[0] = Redraw();
                for (var i = 0; i < Children.Count; ++i)
                    r[i + 1] = Children[i].PrepareRenderMesh();
                CachedRenderMesh = Mesh.Merge(r);
            }

            return CachedRenderMesh;
        }

	}

}