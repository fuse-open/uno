using System;

namespace Uno.Compiler.API.Domain.UXL
{
    [Flags]
    public enum UxlTypeFlags : byte
    {
        /* UxlDocumentEntity */
        Elements = 1 << 0,
        CopyFiles = 1 << 1,
        ImageFiles = 1 << 2,

        /* UxlType */
        Methods = 1 << 4,
        HasCondition = 1 << 5,
        IsDefault = 1 << 6,
    }
}