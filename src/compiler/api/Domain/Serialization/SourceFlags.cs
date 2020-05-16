using System;

namespace Uno.Compiler.API.Domain.Serialization
{
    [Flags]
    public enum SourceFlags : byte
    {
        Path = 1 << 0,
        Line = 1 << 1,
        Column = 1 << 2,
        Length = 1 << 3,
        Flag = 1 << 4,
        Marker = 1 << 5 | 1 << 7,
        MarkerMask = 0xE0
    }
}