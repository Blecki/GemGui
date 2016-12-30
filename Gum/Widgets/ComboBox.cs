﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gum.Widgets
{
    public class ComboBox : Widget
    {
        public List<String> Items = new List<String>();

        private int _selectedIndex = 1;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;

                if (_selectedIndex < 0 || _selectedIndex >= Items.Count)
                    Text = "";
                else
                    Text = Items[_selectedIndex];

                if (Root != null)
                    Root.SafeCall(OnSelectedIndexChanged);

                Invalidate();
            }
        }

        public Action OnSelectedIndexChanged = null;

        public String SelectedItem
        {
            get
            {
                if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
                    return Items[SelectedIndex];
                else return null;
            }
        }

        private Widget SelectorPopup = null;

        public override void Construct()
        {
            if (String.IsNullOrEmpty(Graphics)) Graphics = "combo-down";

            OnClick += (args) =>
                {
                    var interior = GetDrawableInterior();
                    var clickX = args.X - interior.X - interior.Width;
                    if (clickX >= 0) // Clicked the button.
                    {
                        if (SelectorPopup != null)
                            Root.DestroyWidget(SelectorPopup);
                        else
                        {
                            var childRect = new Rectangle(Rect.X, Rect.Y + Rect.Height, Rect.Width, Rect.Width);
                            var listView = Root.CreateWidget(new ListView
                                {
                                    Rect = childRect,
                                    TextSize = TextSize,
                                    Border = "border-thin"
                                }) as ListView;

                            //Todo: Size listview to set number of visible items.
                            listView.Items.AddRange(Items);
                            listView.SelectedIndex = SelectedIndex;
                            listView.Layout();

                            listView.OnSelectedIndexChanged += () =>
                                {
                                    if (SelectorPopup != null)
                                    {
                                        var newSelection = listView.SelectedIndex;
                                        Root.DestroyWidget(SelectorPopup);
                                        SelectedIndex = newSelection;
                                        SelectorPopup = null;
                                    }
                                };

                            listView.OnPopupClose += () =>
                                {
                                    SelectorPopup = null;
                                };

                            SelectorPopup = listView;
                            Root.ShowPopup(SelectorPopup);
                        }
                    }
                };
        }

        public override Point GetBestSize()
        {
            var baseSize = new Point(0, Root.GetTileSheet(Font).TileHeight * TextSize);
            
            var gfx = Root.GetTileSheet(Graphics);
            if (baseSize.X < gfx.TileWidth) baseSize.X = gfx.TileWidth;
            if (baseSize.Y < gfx.TileHeight) baseSize.Y = gfx.TileHeight;
            
            if (!String.IsNullOrEmpty(Border))
            {
                var border = Root.GetTileSheet(Border);
                baseSize.X += border.TileWidth * 2;
                baseSize.Y += border.TileHeight * 2;
            }           
            
            return baseSize;
        }

        public override Rectangle GetDrawableInterior()
        {
            // Ensure text doesn't draw over down arrow.
            var gfx = Root.GetTileSheet(Graphics);
            return base.GetDrawableInterior().Interior(0, 0, gfx.TileWidth, 0);
        }

        protected override Mesh Redraw()
        {
            var gfx = Root.GetTileSheet(Graphics);
            var interior = GetDrawableInterior();
            var downArrow = Mesh.CreateSpriteQuad()
               .Tile(gfx, 0)
               .Translate(interior.X + interior.Width, interior.Y);
            return Mesh.Merge(base.Redraw(), downArrow);
        }
    }
}
