using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Gum
{
    /// <summary>
    /// An individual element in the GUI.
    /// </summary>
    public partial class Widget
    {
        public Rectangle Rect = new Rectangle(0, 0, 0, 0);

        public Point MinimumSize = new Point(0, 0);
        public Point MaximumSize = new Point(int.MaxValue, int.MaxValue);
        public AutoLayout AutoLayout = AutoLayout.None;
        public int Padding = 2;
        public int TopMargin = 0;

        internal List<Widget> Children = new List<Widget>();
        public Widget Parent { get; private set; }
        public Root Root { get; internal set; }

        public bool Transparent = false;

        private Vector4? _backgroundColor = null;
        public Vector4 BackgroundColor
        {
            get
            {
                if (_backgroundColor.HasValue) return _backgroundColor.Value;
                else if (Parent != null) return Parent.BackgroundColor;
                else return Vector4.One;
            }
            set { _backgroundColor = value; }
        }

        public TileReference Background = null;
        public String Graphics = null;
        public String Border = null;

        private String _Font = null;
        public String Font
        {
            get
            {
                if (!String.IsNullOrEmpty(_Font)) return _Font;
                else if (Parent != null) return Parent.Font;
                else return "font";
            }
            set { _Font = value; }
        }

        private Vector4? _textColor = null;
        public Vector4 TextColor
        {
            get
            {
                if (_textColor.HasValue) return _textColor.Value;
                else if (Parent != null) return Parent.TextColor;
                else return new Vector4(0, 0, 0, 1);
            }
            set { _textColor = value; }
        }

        public String Text = null;
        public HorizontalAlign TextHorizontalAlign = HorizontalAlign.Left;
        public VerticalAlign TextVerticalAlign = VerticalAlign.Top;
        public int TextSize = 1; // Todo: Floating point text size?
        public String Tooltip = null;
        public Action<Widget, InputEventArgs> OnMouseEnter = null;
        public Action<Widget, InputEventArgs> OnMouseLeave = null;
        public Action<Widget, InputEventArgs> OnClick = null;
        public Action<Widget> OnGainFocus = null;
        public Action<Widget> OnLoseFocus = null;
        public Action<Widget, InputEventArgs> OnKeyPress = null;
        public Action<Widget, InputEventArgs> OnKeyDown = null;
        public Action<Widget, InputEventArgs> OnKeyUp = null;
        public Action<Widget> OnPopupClose = null;
        public Action<Widget> OnUpdateWhileFocus = null;

        private Mesh CachedRenderMesh = null;

        

        public Widget() { }

        internal void _Construct(Root Root)
        {
            this.Root = Root;
            this.Construct();
        }

        public virtual void Construct() { }

        public Widget FindWidgetAt(int x, int y)
        {
            foreach (var child in Children.Reverse<Widget>())
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
            Invalidate();

            return child;
        }

        public void RemoveChild(Widget child)
        {
            Children.Remove(child);
            child.Parent = null;
            Invalidate();
        }

        /// <summary>
        /// Check to see if the widget is a child, grandchild, etc of another widget.
        /// </summary>
        /// <param name="Ancestor"></param>
        /// <returns>True if ancestor appears in the parent chain of this widget</returns>
        public bool IsChildOf(Widget Ancestor)
        {
            if (Object.ReferenceEquals(Ancestor, Parent)) return true;
            if (Parent != null) return Parent.IsChildOf(Ancestor);
            return false;
        }

        public void Close()
        {
            Root.DestroyWidget(this);
        }

        public virtual Rectangle GetDrawableInterior()
        {
            if (!String.IsNullOrEmpty(Border))
            {
                var tileSheet = Root.GetTileSheet(Border);
                return Rect.Interior(tileSheet.TileWidth, tileSheet.TileHeight,
                    tileSheet.TileWidth, tileSheet.TileHeight);
            }
            else
                return Rect;
        }
        
        public virtual Point GetBestSize()
        {
            var size = new Point(0, 0);
            if (!String.IsNullOrEmpty(Text))
            {
                var font = Root.GetTileSheet(Font);
                size = new Point(font.TileWidth * TextSize * Text.Length,
                    font.TileHeight * TextSize);
            }

            if (!String.IsNullOrEmpty(Border))
            {
                var border = Root.GetTileSheet(Border);
                size = new Point(size.X + border.TileWidth + border.TileWidth,
                    size.Y + border.TileHeight + border.TileHeight);
            }

            return size;
        }

        protected virtual Mesh Redraw()
        {
            if (Transparent)
                return Mesh.EmptyMesh();

            var result = new List<Mesh>();

            if (Background != null)
                result.Add(Mesh.CreateSpriteQuad()
                    .Transform(Matrix.CreateScale(Rect.Width, Rect.Height, 1.0f))
                    .Transform(Matrix.CreateTranslation(Rect.X, Rect.Y, 0.0f))
                    .Colorize(BackgroundColor)
                    .Texture(Root.TileSheets[Background.Sheet].TileMatrix(Background.Tile)));

            if (!String.IsNullOrEmpty(Border))
            {
                //Create a 'scale 9' background 
                result.Add(
                    Mesh.CreateScale9Background(Rect, Root.TileSheets[Border])
                    .Colorize(BackgroundColor));
            }

            // Add text label
            if (!String.IsNullOrEmpty(Text))
            {
                var stringMeshSize = new Rectangle();
                var font = Root.GetTileSheet(Font);
                var textSize = TextSize;
                var stringMesh = Mesh.CreateStringMesh(
                    Text,
                    font,
                    new Vector2(font.TileWidth * textSize, font.TileHeight * textSize),
                    out stringMeshSize)
                    .Colorize(TextColor);

                var drawableArea = GetDrawableInterior();

                var textDrawPos = Vector2.Zero;

                switch (TextHorizontalAlign)
                {
                    case HorizontalAlign.Left:
                        textDrawPos.X = drawableArea.X;
                        break;
                    case HorizontalAlign.Right:
                        textDrawPos.X = drawableArea.X + drawableArea.Width - stringMeshSize.Width;
                        break;
                    case HorizontalAlign.Center:
                        textDrawPos.X = drawableArea.X + ((drawableArea.Width - stringMeshSize.Width) / 2);
                        break;
                }

                switch (TextVerticalAlign)
                {
                    case VerticalAlign.Top:
                        textDrawPos.Y = drawableArea.Y;
                        break;
                    case VerticalAlign.Bottom:
                        textDrawPos.Y = drawableArea.Y + drawableArea.Height - stringMeshSize.Height;
                        break;
                    case VerticalAlign.Center:
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