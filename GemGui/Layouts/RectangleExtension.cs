using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GemGui
{
    public static class RectangleExtension
    {
        public static Microsoft.Xna.Framework.Rectangle Interior(this Microsoft.Xna.Framework.Rectangle r, int Left, int Top, int Right, int Bottom)
        {
            return new Microsoft.Xna.Framework.Rectangle(r.X + Left, r.Y + Top,
                r.Width - Left - Right, r.Height - Top - Bottom);
        }
    }
}


