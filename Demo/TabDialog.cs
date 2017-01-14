using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Gum;

namespace GemGuiTest
{
    public class TabDialog : Widget
    {
        public override void Construct()
        {
            //Set size and center on screen.
            Rect = Root.VirtualScreen.Interior(16, 16, 16, 16);

            Border = "border-fancy";

            // Create a close button in the bottom right.
            AddChild(new Widget
            {
                Text = "CLOSE",
                TextHorizontalAlign = HorizontalAlign.Center,
                TextVerticalAlign = VerticalAlign.Center,
                Border = "border-button",
                TextSize = 2,
                OnClick = (sender, args) => 
                    {
                        this.Close();
                    },
                AutoLayout = AutoLayout.FloatBottomRight
            });

            var panel = AddChild(new Gum.Widgets.TabPanel
            {
                AutoLayout = AutoLayout.DockFill,
                TextSize = 4,
                OnLayout = (sender) => sender.Rect.Height -= 36,
            }) as Gum.Widgets.TabPanel;

            var grid = panel.AddTab("FOO", new Gum.Widgets.TextGrid
            {
                TextSize = 2,
                Border = "border-thin",
            }) as Gum.Widgets.TextGrid;

            var grid2 = panel.AddTab("BAR", new Gum.Widgets.TextGrid
            {
                TextSize = 2,
                Border = "border-thin"
            }) as Gum.Widgets.TextGrid;
            
            Layout();

            grid.SetString("TEST FOO");
            grid2.SetString("TEST BAR");
        }
    }
}
