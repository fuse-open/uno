using System;

namespace Uno.Compiler.API.Domain
{
    [Flags]
    public enum FieldModifiers : byte
    {
        Const = 1 << 0,
        ReadOnly = 1 << 1,
        Volatile = 1 << 2,
    }
}