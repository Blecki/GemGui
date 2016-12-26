using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GemGui.Widgets
{
    public class CheckBox : Widget
    {
        private bool _checkState = false;
        public bool CheckState 
        {
            get { return _checkState; }
            set { _checkState = value; Invalidate(); }
        }

        internal override void OnConstruct()
        {
            Properties.OnClick += (args) => { CheckState = !CheckState; };
            Properties.TextVerticalAlign = WidgetProperties.VerticalAlign.Center;
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
                .TransformTexture(Root.TileSheets["checkbox"].TileMatrix(CheckState ? 1 : 0));

            return Mesh.Merge(baseMesh, checkMesh);
        }
    }
}
