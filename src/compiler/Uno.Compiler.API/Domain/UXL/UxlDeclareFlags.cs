using System;

namespace Uno.Compiler.API.Domain.UXL
{
    [Flags]
    public enum UxlDeclareFlags : byte
    {
        TypeMask = 0x7f,
        HasCondition = 1 << 7,
    }
}