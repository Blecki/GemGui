using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GemGui
{
    public struct BorderLayout
    {
        public enum Sides
        {
            Top,
            Right,
            Bottom,
            Left,
            Fill
        }

        Rectangle CoverageArea;
        int Padding;

        public BorderLayout(Rectangle Area, int Padding)
        {
            CoverageArea = Layout.Center(new Rectangle(0,0,Area.Width - 2 * Padding, Area.Height - 2 * Padding), Area);
            this.Padding = Padding;            
        }

        public Rectangle Position(Sides Side, int Size)
        {
            Rectangle R;

            switch (Side)
            {
                case Sides.Top:
                    R = new Rectangle(CoverageArea.X, CoverageArea.Y, CoverageArea.Width, Size);
                    CoverageArea.Y += Size + Padding; CoverageArea.Height -= Size + Padding;
                    break;
                case Sides.Right:
                    R = new Rectangle(CoverageArea.X + CoverageArea.Width - Size, CoverageArea.Y, Size, CoverageArea.Height);
                    CoverageArea.Width -= Size + Padding;
                    break;
                case Sides.Bottom:
                    R = new Rectangle(CoverageArea.X, CoverageArea.Y + CoverageArea.Height - Size, CoverageArea.Width, Size);
                    CoverageArea.Height -= Size + Padding;
                    break;
                case Sides.Left:
                    R = new Rectangle(CoverageArea.X, CoverageArea.Y, Size, CoverageArea.Height);
                    CoverageArea.X += Size + Padding; CoverageArea.Width -= Size + Padding;
                    break;
                case Sides.Fill:
                    R = CoverageArea;
                    CoverageArea = new Rectangle(0, 0, 0, 0);
                    break;
                default :
                    R = new Rectangle(0, 0, 0, 0);
                    break;
            }

            return R;
        }
    }
}
