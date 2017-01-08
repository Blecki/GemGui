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
            Rect = new Rectangle(0, 0, Root.GuiTexture.Width + 4, Root.GuiTexture.Height + 4);
            AutoLayout = AutoLayout.FloatCenter;
            MinimumSize = new Point(Rect.Width, Rect.Height);

            Border = "border-thin";

            AddChild(new Widget
            {
                Text = "CLOSE",
                TextHorizontalAlign = HorizontalAlign.Center,
                TextVerticalAlign = VerticalAlign.Center,
                Border = "border-button",
                TextSize = 2,
                OnClick = (sender, args) => this.Close(),
                AutoLayout = AutoLayout.FloatBottomRight
            });
        }

        protected override Mesh Redraw()
        {
            var drawable = this.GetDrawableInterior();
            var mesh = Mesh.Quad()
                .Scale(drawable.Width, drawable.Height)
                .Translate(drawable.X, drawable.Y);

            return Mesh.Merge(base.Redraw(), mesh);
        }
    }
}
