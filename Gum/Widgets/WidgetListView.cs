﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gum.Widgets
{
    /// <summary>
    /// Display a list of items with a scrollbar. Items are selected by clicking.
    /// Like a normal list view, except items are widgets instead of strings.
    /// </summary>
    public class WidgetListView : Widget
    {
        private int _selectedIndex = 1;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set 
            { 
                _selectedIndex = value;
                if (_selectedIndex < 0) _selectedIndex = 0;
                if (_selectedIndex >= Children.Count - 1) _selectedIndex = Children.Count - 2;
                if (Root != null) 
                    Root.SafeCall(OnSelectedIndexChanged, this);
                Invalidate();
            }
        }

        public Action<Widget> OnSelectedIndexChanged = null;
        public Vector4 SelectedItemBackgroundColor = new Vector4(1, 0, 0, 1);

        public Widget SelectedItem
        {
            get
            {
                if (SelectedIndex >= 0 && SelectedIndex < Children.Count - 1)
                    return Children[SelectedIndex + 1];
                else return null;
            }
        }

        private VerticalScrollBar ScrollBar;
        public int ItemHeight = 32;

        public override void Construct()
        {
            if (String.IsNullOrEmpty(Border)) Border = "border-one";

            ScrollBar = new VerticalScrollBar
            {
                OnScroll = (sender) => { this.Invalidate(); },
                AutoLayout = AutoLayout.DockRight
            };

            AddChild(ScrollBar);

            OnClick += (sender, args) =>
                {
                    SelectedIndex = ScrollBar.ScrollPosition + ((args.Y - GetDrawableInterior().Y) / ItemHeight);
                };
        }

        public void AddItem(Widget Item)
        {
            Item.AutoLayout = Gum.AutoLayout.None;
            Item.OnClick += (sender, args) => this.OnClick(sender, args);
            AddChild(Item);
        }

        public override Point GetBestSize()
        {
            if (Rect.Width == 0 || Rect.Height == 0) return new Point(128, 128); // Arbitrary!
            return new Point(Rect.Width, Rect.Height);
        }

        protected override Mesh Redraw()
        {
            var font = Root.GetTileSheet(Font);
            var drawableInterior = GetDrawableInterior();
            drawableInterior.Width = (Children[0].Rect.Left - drawableInterior.Left - Padding.Right);
            var itemsThatFit = drawableInterior.Height / ItemHeight;

            // Update scrollbar scroll area.
            if (itemsThatFit > Children.Count - 1) ScrollBar.ScrollArea = 0;
            else ScrollBar.ScrollArea = Children.Count - itemsThatFit;

            var topItem = ScrollBar.ScrollPosition;
            if (topItem < 0) topItem = 0;

            var topPos = drawableInterior.Y;

            for (int i = 1; i < Children.Count; ++i)
            {
                Children[i].Hidden = true;
                Children[i].BackgroundColor = new Vector4(1, 1, 1, 1);
                Children[i].Invalidate();
            }

            for (int i = 0; i < itemsThatFit && (topItem + i) < Children.Count; ++i)
            {
                Children[topItem + i + 1].Rect = new Rectangle(drawableInterior.Left, topPos, drawableInterior.Width,
                    ItemHeight);
                Children[topItem + i + 1].Layout();
                Children[topItem + i + 1].Hidden = false;
                topPos += ItemHeight;
            }

            if (SelectedItem != null)
                SelectedItem.BackgroundColor = SelectedItemBackgroundColor;

            return base.Redraw();
        }
    }
}