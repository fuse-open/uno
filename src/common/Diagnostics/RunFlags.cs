using System;

namespace Uno.Diagnostics
{
    [Flags]
    public enum RunFlags
    {
        Colors = 1 << 0,
        NoOutput = 1 << 1,
        NoThrow = 1 << 2,
        Compact = 1 << 3,
    }
}