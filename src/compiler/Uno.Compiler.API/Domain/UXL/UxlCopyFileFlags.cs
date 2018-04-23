using System;

namespace Uno.Compiler.API.Domain.UXL
{
    [Flags]
    public enum UxlCopyFileFlags : byte
    {
        FlagsMask = 0xf,
        HasCondition = 1 << 4,
        HasTargetName = 1 << 5,
        HasType = 1 << 6,
    }
}
