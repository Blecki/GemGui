using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemGui
{
    public enum KeyEvents
    {
        KeyPress,
        KeyDown,
        KeyUp
    }

    public struct KeyEventArgs
    {
        public bool Alt;
        public bool Control;
        public bool Shift;

        public int KeyData;
        public int KeyValue;
    }
}
