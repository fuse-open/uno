using System;

namespace Uno.Compiler.API.Domain.UXL
{
    [Flags]
    public enum UxlImageFileFlags : byte
    {
        HasCondition = 1 << 0,
        HasTargetName = 1 << 1,
        HasTargetWidth = 1 << 2,
        HasTargetHeight = 1 << 3,
    }
}