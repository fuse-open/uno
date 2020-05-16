using System;

namespace Uno.Compiler.API.Domain.UXL
{
    [Flags]
    public enum UxlImplementationFlags : byte
    {
        TypeMask = 0xf,
        HasCondition = 1 << 4,
        IsDefault = 1 << 5,
    }
}