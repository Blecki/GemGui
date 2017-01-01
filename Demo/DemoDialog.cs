using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Gum;

namespace GemGuiTest
{
    public class DemoDialog : Widget
    {
        public override void Construct()
        {
            //Set size and center on screen.
            Rect = Root.VirtualScreen.Interior(16, 16, 16, 16);

            Border = "border-fancy";

            // Create a close button in the bottom right.
            AddChild(Root.CreateWidget(new Widget
            {
                Text = "CLOSE",
                TextHorizontalAlign = HorizontalAlign.Center,
                TextVerticalAlign = VerticalAlign.Center,
                Border = "border-button",
                TextSize = 2,
                OnClick = (sender, args) => this.Close(),
                AutoLayout = AutoLayout.FloatBottomRight
            }));

            AddChild(Root.CreateWidget(new Gum.Widgets.EditableTextField
            {
                Text = "edit me",
                AutoLayout = AutoLayout.DockTop
            }));

            AddChild(Root.CreateWidget(new Gum.Widgets.EditableTextField
            {
                Text = "edit me too",
                TextSize = 2,
                AutoLayout = AutoLayout.DockTop
            }));

            AddChild(Root.CreateWidget(new Gum.Widgets.CheckBox
            {
                Text = "CHECKBOX!",
                TextSize = 2,
                AutoLayout = AutoLayout.DockTop
            }));

            AddChild(Root.CreateWidget(new Gum.Widgets.CheckBox
            {
                Text = "RADIO BUTTON!",
                Graphics = "radiobutton",
                TextSize = 2,
                AutoLayout = AutoLayout.DockTop
            }));

            var listView = AddChild(Root.CreateWidget(new Gum.Widgets.ListView
            {
                Rect = new Rectangle(0, 0, 256, 256),
                TextSize = 2,
                AutoLayout = AutoLayout.DockLeft
            })) as Gum.Widgets.ListView;
            for (int i = 0; i < 40; ++i)
                listView.Items.Add(String.Format("Item number {0}", i));
            listView.SelectedIndex = 10;


            var comboBox = AddChild(Root.CreateWidget(new Gum.Widgets.ComboBox
            {
                TextSize = 2,
                AutoLayout = AutoLayout.DockTop
            })) as Gum.Widgets.ComboBox;
            for (int i = 0; i < 40; ++i)
                comboBox.Items.Add(String.Format("Combo number {0}", i));
            comboBox.SelectedIndex = 10;

            var normalMouseButton = AddChild(Root.CreateWidget(new Widget
            {
                Background = new TileReference("mouse", 0),
                OnClick = (sender, args) => { Root.MousePointer = new MousePointer("mouse", 1.0f, 0); },
                MinimumSize = new Point(Root.GetTileSheet("mouse").TileWidth, Root.GetTileSheet("mouse").TileHeight),
                MaximumSize = new Point(Root.GetTileSheet("mouse").TileWidth, Root.GetTileSheet("mouse").TileHeight),
                Tooltip = "Click to switch to arrow",
                AutoLayout = AutoLayout.DockTop
            }));

            var hourglassMouseButton = AddChild(Root.CreateWidget(new Widget
            {
                Background = new TileReference("mouse", 14),
                OnClick = (sender, args) =>
                {
                    Root.MousePointer = new MousePointer("mouse", 4.0f, 14, 15, 16, 17, 18, 19);
                },
                MinimumSize = new Point(Root.GetTileSheet("mouse").TileWidth, Root.GetTileSheet("mouse").TileHeight),
                MaximumSize = new Point(Root.GetTileSheet("mouse").TileWidth, Root.GetTileSheet("mouse").TileHeight),
                Tooltip = "Click to switch to hourglass",
                AutoLayout = AutoLayout.DockTop
            }));

            Layout();
        }
    }
}
