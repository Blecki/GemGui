using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gum
{
    public static class Layout
    {
        public static void ShrinkAndCenter(Widget Widget, Rectangle Area)
        {
            var bestSize = Widget.GetBestSize();
            Widget.Rect = new Rectangle(
                Area.X + (Area.Width / 2) - (bestSize.X / 2),
                Area.Y + (Area.Height / 2) - (bestSize.Y / 2),
                bestSize.X, bestSize.Y);
            Widget.Layout();
        }

        public static void Center(Widget Widget, Rectangle Area)
        {
            var bestSize = new Point(Widget.Rect.Width, Widget.Rect.Height);
            Widget.Rect = new Rectangle(
                Area.X + (Area.Width / 2) - (bestSize.X / 2),
                Area.Y + (Area.Height / 2) - (bestSize.Y / 2),
                bestSize.X, bestSize.Y);
            Widget.Layout();
        }
    }
}
