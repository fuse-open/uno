using System;

namespace Uno.Compiler.Backends.CPlusPlus
{
    [Flags]
    public enum CallFlags
    {
        Constrained = 1 << 0,
        Extension = 1 << 1,
    }
}