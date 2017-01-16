using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gum.Widgets
{
    /// <summary>
    /// Display a set of widgets, controlling visibility using tabs.
    /// </summary>
    public class TabPanel : Widget
    {
        private class TabButton : Widget
        {
            protected override Mesh Redraw()
            {
                Rectangle drop;
                var border = Root.GetTileSheet(Graphics);
                var bgMesh = Mesh.CreateTabBackground(Rect, border);
                var labelMesh = Mesh.CreateStringMesh(Text, Root.GetTileSheet(Font), new Vector2(TextSize, TextSize), out drop)
                    .Translate(Rect.X + border.TileWidth, Rect.Y + border.TileHeight)
                    .Colorize(TextColor);
                return Mesh.Merge(bgMesh, labelMesh);                
            }

            public override Point GetBestSize()
            {
                var font = Root.GetTileSheet(Font);
                var border = Root.GetTileSheet(Graphics);
                var labelSize = font.MeasureString(Text).Scale(TextSize);
                return new Point(labelSize.X + (2 * border.TileWidth), labelSize.Y + border.TileHeight);
            }
        }

        private List<Widget> TabPanels = new List<Widget>();
        private List<Widget> TabButtons = new List<Widget>();
        public int TabPadding = 4;

        private int _selectedTab = 1;
        public int SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                _selectedTab = value;

                foreach (var child in TabPanels)
                    child.Hidden = true;
                TabPanels[_selectedTab].Hidden = false;

                if (Root != null)
                    Root.SafeCall(OnSelectedTabChanged, this);

                Invalidate();
            }
        }

        public Action<Widget> OnSelectedTabChanged = null;
        public Vector4 SelectedTabColor = new Vector4(1, 0, 0, 1);

        public override void Construct()
        {
        }

        /// <summary>
        /// Override layout to disable layout engine on children - just make them fill the space.
        /// </summary>
        public override void Layout()
        {
            Root.SafeCall(this.OnLayout, this);

            if (TabButtons.Count == 0) return;

            var tabPosition = new Point(Rect.X, Rect.Y);
            foreach (var tabButton in TabButtons)
            {
                var bestSize = tabButton.GetBestSize();
                tabButton.Rect = new Rectangle(tabPosition.X, tabPosition.Y, bestSize.X, bestSize.Y);
                tabPosition.X += bestSize.X + TabPadding;
                tabButton.Invalidate();
            }

            var tabSize = TabButtons[0].GetBestSize();
            // Todo: Honor all margins.
            var interior = GetDrawableInterior().Interior(0, tabSize.Y, 0, 0);
            foreach (var child in TabPanels)
            {
                child.Rect = interior;
                child.Layout();
            }
        }

        public Widget AddTab(String Name, Widget Tab)
        {
            var tabPosition = new Point(Rect.X, Rect.Y);
            if (TabButtons.Count > 0)
                tabPosition.X = TabButtons[TabButtons.Count - 1].Rect.Right + TabPadding;

            var tabIndex = TabButtons.Count;
            var tabButton = AddChild(new TabButton
            {
                Text = Name,
                Graphics = "border-one",
                OnClick = (sender, args) => SelectedTab = tabIndex,
                TextSize = TextSize,
                TextColor = TextColor
            });

            var tabSize = tabButton.GetBestSize();
            tabButton.Rect = new Rectangle(tabPosition.X, tabPosition.Y, tabSize.X, tabSize.Y);
            TabButtons.Add(tabButton);

            AddChild(Tab);
            TabPanels.Add(Tab);

            if (TabButtons.Count == 1)
                Tab.Hidden = false;
            else
                Tab.Hidden = true;
            
            return Tab;
        }        
    }
}
