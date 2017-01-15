﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gum
{
    public struct Margin
    {
        public int Top;
        public int Bottom;
        public int Right;
        public int Left;

        public static Margin Zero { get { return new Margin { Top = 0, Bottom = 0, Right = 0, Left = 0 }; } }

        public Margin(int Top, int Bottom, int Left, int Right)
        {
            this.Top = Top;
            this.Bottom = Bottom;
            this.Left = Left;
            this.Right = Right;
        }
    }
}
