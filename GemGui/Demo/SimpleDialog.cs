using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GemGui.Demo
{
    public class SimpleDialog : Widget
    {
        internal override void OnConstruct()
        {
            //Set size and center on screen.
            Rect = Root.VirtualScreen.Interior(16, 16, 16, 16);

            Properties.Border = "fancy-border";
            Properties.Transparent = false;

            // Create a close button in the bottom right.
            var internalArea = GetDrawableInterior();
            var dockLayout = new BorderLayout(internalArea, 2);

            AddChild(Root.CreateUIItem(
                Layout.Float(new Rectangle(0, 0, 70, 24), internalArea, Layout.Placements.BottomRight, 0),
                new WidgetProperties()
                {
                    Text = "CLOSE",
                    TextHorizontalAlign = WidgetProperties.HorizontalAlign.Center,
                    TextVerticalAlign = WidgetProperties.VerticalAlign.Center,
                    Border = "button",
                    TextSize = 2,
                    OnClick = (args) =>
                        {
                            this.Close();
                        }
                }));

            AddChild(Root.CreateUIItem<Widgets.EditableTextField>(
                dockLayout.Position(BorderLayout.Sides.Top, 24),
                new WidgetProperties
                {
                    Text = "edit me"
                }));

            AddChild(Root.CreateUIItem<Widgets.EditableTextField>(
                dockLayout.Position(BorderLayout.Sides.Top, 24),
                new WidgetProperties
                {
                    Text = "edit me",
                    TextSize = 2
                }));

            AddChild(Root.CreateUIItem<Widgets.CheckBox>(
                dockLayout.Position(BorderLayout.Sides.Top, 18),
                new WidgetProperties
                {
                    Text = "CHECKBOX!",
                    Border = null
                }));
        }
    }
}
