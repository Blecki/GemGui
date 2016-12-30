using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Gum;

namespace GemGuiTest
{
    public class ShowTextureDialog : Widget
    {
        public override void Construct()
        {
            //Set size and center on screen.
            Rect = Root.VirtualScreen.Interior(16, 16, 16, 16);

            Border = "border-thin";

            // Create a close button in the bottom right.
            var internalArea = GetDrawableInterior();
            var dockLayout = new DockLayout(internalArea, 2);

            dockLayout.Float(AddChild(Root.CreateWidget(new Widget
            {
                Text = "CLOSE",
                TextHorizontalAlign = HorizontalAlign.Center,
                TextVerticalAlign = VerticalAlign.Center,
                Border = "border-button",
                TextSize = 2,
                OnClick = (args) => this.Close()
            })), DockLayout.Sides.Bottom, DockLayout.Sides.Right);
   
        }

        protected override Mesh Redraw()
        {
            var drawable = this.GetDrawableInterior();
            var mesh = Mesh.CreateSpriteQuad()
                .Transform(Matrix.CreateScale(drawable.Width, drawable.Height, 1.0f))
                .Transform(Matrix.CreateTranslation(drawable.X, drawable.Y, 0.0f));

            return Mesh.Merge(base.Redraw(), mesh);
        }
    }
}
