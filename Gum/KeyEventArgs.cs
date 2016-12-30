using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gum
{
    public enum InputEvents
    {
        KeyPress,
        KeyDown,
        KeyUp,
        MouseMove,
        MouseEnter,
        MouseLeave,
        MouseHover,
        MouseClick
    }

    public struct InputEventArgs
    {
        public bool Alt;
        public bool Control;
        public bool Shift;

        public int KeyValue;
        public int X;
        public int Y;
    }
}
