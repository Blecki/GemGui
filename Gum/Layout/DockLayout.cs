using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gum
{
    public struct DockLayout
    {
        public enum Sides
        {
            Top,
            Right,
            Bottom,
            Left,
            Center,
        }

        [Flags]
        public enum Placements
        {
            Top = 1,
            Right = 2,
            Bottom = 4,
            Left = 8,
            TopRight = Top | Right,
            TopLeft = Top | Left,
            BottomRight = Bottom | Right,
            BottomLeft = Bottom | Left,
        }

        Rectangle CoverageArea;
        int Padding;

        public DockLayout(Rectangle Area, int Padding)
        {
            CoverageArea = Area.Interior(Padding, Padding, Padding, Padding);
            this.Padding = Padding;            
        }

        // Todo : 'DockNew' function - remove messy create-and-add process.
        public Widget Dock(Widget Widget, Sides Side)
        {
            Rectangle R;
            var bestSize = Widget.GetBestSize();

            switch (Side)
            {
                case Sides.Top:
                    R = new Rectangle(CoverageArea.X, CoverageArea.Y, CoverageArea.Width, bestSize.Y);
                    CoverageArea.Y += bestSize.Y + Padding; 
                    CoverageArea.Height -= bestSize.Y + Padding;
                    break;
                case Sides.Right:
                    R = new Rectangle(CoverageArea.X + CoverageArea.Width - bestSize.X, CoverageArea.Y, bestSize.X, CoverageArea.Height);
                    CoverageArea.Width -= bestSize.X + Padding;
                    break;
                case Sides.Bottom:
                    R = new Rectangle(CoverageArea.X, CoverageArea.Y + CoverageArea.Height - bestSize.Y, CoverageArea.Width, bestSize.Y);
                    CoverageArea.Height -= bestSize.Y + Padding;
                    break;
                case Sides.Left:
                    R = new Rectangle(CoverageArea.X, CoverageArea.Y, bestSize.X, CoverageArea.Height);
                    CoverageArea.X += bestSize.X + Padding; CoverageArea.Width -= bestSize.X + Padding;
                    break;
                case Sides.Center:
                    R = CoverageArea;
                    CoverageArea = new Rectangle(0, 0, 0, 0);
                    break;
                default :
                    R = new Rectangle(0, 0, 0, 0);
                    break;
            }

            Widget.Rect = R;
            Widget.Layout();
            return Widget;
        }

        public void Float(Widget Widget, Sides First, Sides Second)
        {
            Dock(Widget, First); 

            // Override one dimension based on secondary side.
            var bestSize = Widget.GetBestSize();

            switch (Second)
            {
                case Sides.Top:
                    Widget.Rect = new Rectangle(Widget.Rect.X, Widget.Rect.Y, Widget.Rect.Width, bestSize.Y);
                    break;
                case Sides.Bottom:
                    Widget.Rect = new Rectangle(Widget.Rect.X, Widget.Rect.Y + Widget.Rect.Height - bestSize.Y,
                        Widget.Rect.Width, bestSize.Y);
                    break;
                case Sides.Left:
                    Widget.Rect = new Rectangle(Widget.Rect.X, Widget.Rect.Y, bestSize.X, Widget.Rect.Height);
                    break;
                case Sides.Right:
                    Widget.Rect = new Rectangle(Widget.Rect.X + Widget.Rect.Width - bestSize.X, Widget.Rect.Y,
                        bestSize.X, Widget.Rect.Height);
                    break;
                default:
                    break;
            }

            Widget.Layout();
        }

        public void Center(Widget Widget)
        {
            Layout.Center(Widget, CoverageArea);
        }
    }
}
