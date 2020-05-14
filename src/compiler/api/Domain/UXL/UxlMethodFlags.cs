using System;

namespace Uno.Compiler.API.Domain.UXL
{
    [Flags]
    public enum UxlMethodFlags : byte
    {
        /* UxlEntity */
        Elements = 1 << 0,
        CopyFiles = 1 << 1,
        ImageFiles = 1 << 2,

        /* UxlMethod */
        MethodProperties = 1 << 4,
        Implementations = 1 << 5,
        HasCondition = 1 << 6,
        IsDefault = 1 << 7,
    }
}