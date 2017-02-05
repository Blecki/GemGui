using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Gum;

namespace GemGuiTest
{
    public class BorderTilingDialog : Widget
    {
        public override void Construct()
        {
            //Set size and center on screen.
            Rect = Root.VirtualScreen.Interior(16, 16, 16, 16);

            Border = "tiling-required-border";
            Padding = new Margin(2, 2, 2, 2);

            // Create a close button in the bottom right.
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
            
            Layout();
        }
    }
}
