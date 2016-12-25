using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GemGui.Widgets
{
    public class EditableTextField : Widget
    {
        //Todo: Static method to get best size.

        private int CursorPosition = 0;

        internal override void OnConstruct()
        {
            Properties.Border = "button";
            Properties.TextVerticalAlign = WidgetProperties.VerticalAlign.Center;
            Properties.TextHorizontalAlign = WidgetProperties.HorizontalAlign.Left;
            Properties.OnClick += (args) =>
                {
                    Root.SetFocus(this);
                    Invalidate();

                    //Todo: Detect and place cursor.
                };
            Properties.OnGainFocus += () => this.Invalidate();
            Properties.OnLoseFocus += () => this.Invalidate();
        }

        protected override Mesh Redraw()
        {
            var mesh = base.Redraw();

            if (Object.ReferenceEquals(this, Root.FocusItem))
            {
                var drawableArea = this.GetDrawableInterior();
                var cursorMesh = Mesh.CreateSpriteQuad();
                cursorMesh.Transform(Matrix.CreateScale(2.0f, 10.0f, 1.0f));
                cursorMesh.Transform(Matrix.CreateTranslation(drawableArea.X, drawableArea.Y, 0.0f));
                cursorMesh.TransformTexture(Root.TileSheets["Basic"].TileMatrix(0));
                cursorMesh.Colorize(new Vector4(1, 0, 0, 1));
                mesh = Mesh.Merge(mesh, cursorMesh);                
            }

            return mesh;
        }
    }
}
