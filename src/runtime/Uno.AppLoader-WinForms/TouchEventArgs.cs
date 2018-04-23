using System;

namespace Uno.Support.WinForms
{
    public class TouchEventArgs : EventArgs
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Index { get; set; }
    }
}