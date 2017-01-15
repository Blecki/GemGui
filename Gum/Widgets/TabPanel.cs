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
        public List<String> TabNames = new List<String>();
        public int TabPadding = 4;

        private int _selectedTab = 1;
        public int SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                _selectedTab = value;

                foreach (var child in Children)
                    child.Hidden = true;
                Children[_selectedTab].Hidden = false;

                if (Root != null)
                    Root.SafeCall(OnSelectedTabChanged, this);
                Invalidate();
            }
        }

        public Action<Widget> OnSelectedTabChanged = null;
        public Vector4 SelectedTabColor = new Vector4(1, 0, 0, 1);

        public String SelectedName
        {
            get
            {
                if (SelectedTab >= 0 && SelectedTab < TabNames.Count)
                    return TabNames[SelectedTab];
                else return null;
            }
        }

        public override void Construct()
        {
            if (InteriorMargin.Top == 0)
                InteriorMargin.Top = Root.GetTileSheet(Font).TileHeight * IntegerTextSize;

            OnClick += (sender, args) =>
                {
                    var interior = GetDrawableInterior();
                    var realPoint = new Point(args.X - interior.X, args.Y - interior.Y);
                    if (realPoint.Y > InteriorMargin.Top) return;

                    var tabIndex = 0;
                    foreach (var tab in TabNames)
                    {
                        var tabLength = Root.GetTileSheet(Font).TileWidth * PixelPerfectTextSize * tab.Length;
                        if (realPoint.X < tabLength)
                        {
                            SelectedTab = tabIndex;
                            break;
                        }

                        tabIndex += 1;
                        realPoint.X -= (int)tabLength + TabPadding;
                    }

                    Invalidate();
                };
        }

        /// <summary>
        /// Override layout to disable layout engine on children - just make them fill the space.
        /// </summary>
        public override void Layout()
        {
            Root.SafeCall(this.OnLayout, this);
            var interior = GetDrawableInterior().Interior(InteriorMargin);
            foreach (var child in Children)
            {
                child.Rect = interior;
                child.Layout();
            }
        }

        public Widget AddTab(String Name, Widget Tab)
        {
            AddChild(Tab);
            TabNames.Add(Name);

            if (TabNames.Count == 1)
                Tab.Hidden = false;
            else
                Tab.Hidden = true;

            return Tab;
        }
        
        protected override Mesh Redraw()
        {
            var meshes = new List<Mesh>();
            meshes.Add(base.Redraw());

            var interior = GetDrawableInterior();
            var tx = interior.X;
            foreach (var tab in TabNames)
            {
                var stringMeshSize = new Rectangle();
                var font = Root.GetTileSheet(Font);
                var textSize = TextSize;
                var stringMesh = Mesh.CreateStringMesh(
                    tab,
                    font,
                    new Vector2(font.TileWidth * PixelPerfectTextSize, font.TileHeight * PixelPerfectTextSize),
                    out stringMeshSize)
                    .Colorize(TextColor)
                    .Translate(tx, interior.Y);
                meshes.Add(stringMesh);
                tx += stringMeshSize.Width + TabPadding;
            }

            return Mesh.Merge(meshes.ToArray());
        }
    }
}
