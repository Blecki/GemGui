using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Gum.Widgets
{
    public class CheckBox : Widget
    {
        private bool _checkState = false;
        public bool CheckState 
        {
            get { return _checkState; }
            set { _checkState = value; Invalidate(); }
        }

        public override void Construct()
        {
            OnClick += (sender, args) => { CheckState = !CheckState; };
            TextVerticalAlign = VerticalAlign.Center;

            if (String.IsNullOrEmpty(Graphics))
                Graphics = "checkbox";
        }

        public override Rectangle GetDrawableInterior()
        {
            var baseDrawArea = base.GetDrawableInterior();
            return baseDrawArea.Interior(baseDrawArea.Height + 2, 0, 0, 0);
        }

        protected override Mesh Redraw()
        {
            var baseMesh = base.Redraw();
            var baseDrawArea = base.GetDrawableInterior();

            var checkMesh = Mesh.CreateSpriteQuad()
                .Transform(Matrix.CreateScale(baseDrawArea.Height, baseDrawArea.Height, 1.0f))
                .Transform(Matrix.CreateTranslation(baseDrawArea.X, baseDrawArea.Y, 0.0f))
                .Texture(Root.TileSheets[Graphics].TileMatrix(CheckState ? 1 : 0));

            return Mesh.Merge(baseMesh, checkMesh);
        }

        public override Point GetBestSize()
        {
            var size = new Point(0, 0);
            if (!String.IsNullOrEmpty(Text))
            {
                var font = Root.GetTileSheet(Font);
                size = new Point(font.TileWidth * TextSize * Text.Length,
                    font.TileHeight * TextSize);
            }

            var gfx = Root.GetTileSheet(Graphics);
            return new Point(gfx.TileWidth + size.X, Math.Max(gfx.TileHeight, size.Y));
        }
    }
}
