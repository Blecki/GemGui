﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gum
{
    public partial class Widget
    {
        /// <summary>
        /// Run the layout engine on this widget and all children.
        /// Warning: This operation invalidates the entire tree and potentially moves all children widgets.
        /// </summary>
        public void Layout()
        {
            Root.SafeCall(this.OnLayout, this);
            var inside = GetDrawableInterior().Interior(0, TopMargin, 0, 0);
            foreach (var child in Children)
                inside = LayoutChild(inside, Padding, child);
            Invalidate();
        }

        private static int Clamp(int What, int Min, int Max)
        {
            if (What < Min) return Min;
            if (What > Max) return Max;
            return What;
        }

        private static Point GetClampedChildSize(Widget Child, Point Proposed)
        {
            return new Point(
                Clamp(Proposed.X, Child.MinimumSize.X, Child.MaximumSize.X),
                Clamp(Proposed.Y, Child.MinimumSize.Y, Child.MaximumSize.Y));
        }

        // Todo: 'Flow' options. Pack widgets up in rows until full, flow...
        // Must track rects in each corner used for flow.
        private static Rectangle LayoutChild(Rectangle Inside, int Padding, Widget Child)
        {
            Rectangle newPos;

            var size = Child.GetBestSize();
            
            switch (Child.AutoLayout)
            {
                case AutoLayout.None:
                    newPos = Child.Rect;
                    break;
                case AutoLayout.DockTop:
                    size = GetClampedChildSize(Child, new Point(Inside.Width, size.Y));
                    newPos = new Rectangle(Inside.X, Inside.Y, size.X, size.Y);
                    Inside.Y += size.Y + Padding;
                    Inside.Height -= size.Y + Padding;
                    break;
                case AutoLayout.DockRight:
                    size = GetClampedChildSize(Child, new Point(size.X, Inside.Height));
                    newPos = new Rectangle(Inside.X + Inside.Width - size.X, Inside.Y, size.X, size.Y);
                    Inside.Width -= size.X + Padding;
                    break;
                case AutoLayout.DockBottom:
                    size = GetClampedChildSize(Child, new Point(Inside.Width, size.Y));
                    newPos = new Rectangle(Inside.X, Inside.Y + Inside.Height - size.Y, size.X, size.Y);
                    Inside.Height -= size.Y + Padding;
                    break;
                case AutoLayout.DockLeft:
                    size = GetClampedChildSize(Child, new Point(size.X, Inside.Height));
                    newPos = new Rectangle(Inside.X, Inside.Y, size.X, size.Y);
                    Inside.X += size.X + Padding;
                    Inside.Width -= size.X + Padding;
                    break;
                case AutoLayout.DockFill:
                    size = GetClampedChildSize(Child, new Point(Inside.Width, Inside.Height));
                    newPos = new Rectangle(
                        Inside.X + (Inside.Width / 2) - (size.X / 2),
                        Inside.Y + (Inside.Height / 2) - (size.Y / 2),
                        size.X, size.Y);
                    Inside = new Rectangle(0, 0, 0, 0);
                    break;
                case AutoLayout.FloatCenter:
                    size = GetClampedChildSize(Child, size);
                    newPos = new Rectangle(
                        Inside.X + (Inside.Width / 2) - (size.X / 2),
                        Inside.Y + (Inside.Height / 2) - (size.Y / 2),
                        size.X, size.Y);
                    break;
                case AutoLayout.FloatTop:
                    size = GetClampedChildSize(Child, size);
                    newPos = new Rectangle(Inside.X + (Inside.Width / 2) - (size.X / 2),
                        Inside.Y, size.X, size.Y);
                    break;
                case AutoLayout.FloatBottom:
                    size = GetClampedChildSize(Child, size);
                    newPos = new Rectangle(Inside.X + (Inside.Width / 2) - (size.X / 2),
                        Inside.Bottom - size.Y, size.X, size.Y);
                    break;
                case AutoLayout.FloatLeft:
                    size = GetClampedChildSize(Child, size);
                    newPos = new Rectangle(Inside.X, Inside.Y + (Inside.Height / 2) - (size.Y / 2),
                        size.X, size.Y);
                    break;
                case AutoLayout.FloatRight:
                    size = GetClampedChildSize(Child, size);
                    newPos = new Rectangle(Inside.Right - size.X,
                        Inside.Y + (Inside.Height / 2) - (size.Y / 2),
                        size.X, size.Y);
                    break;
                case AutoLayout.FloatTopRight:
                    size = GetClampedChildSize(Child, size);
                    newPos = new Rectangle(Inside.X + Inside.Width - size.X, Inside.Y, size.X, size.Y);
                    break;
                case AutoLayout.FloatTopLeft:
                    size = GetClampedChildSize(Child, size);
                    newPos = new Rectangle(Inside.X, Inside.Y, size.X, size.Y);
                    break;
                case AutoLayout.FloatBottomRight:
                    size = GetClampedChildSize(Child, size);
                    newPos = new Rectangle(
                        Inside.X + Inside.Width - size.X,
                        Inside.Y + Inside.Height - size.Y, 
                        size.X, size.Y);
                    break;
                case AutoLayout.FloatBottomLeft:
                    size = GetClampedChildSize(Child, size);
                    newPos = new Rectangle(Inside.X, Inside.Y + Inside.Height - size.Y, size.X, size.Y);
                    break;
                default:
                    newPos = new Rectangle(0, 0, 0, 0);
                    break;
            }

            Child.Rect = newPos;
            Child.Layout();

            return Inside;
        }
    }
}