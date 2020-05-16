using System;

namespace Uno.Compiler.API.Domain.UXL
{
    [Flags]
    public enum UxlTemplateFlags : byte
    {
        /* UxlEntity */
        Elements = 1 << 0,
        CopyFiles = 1 << 1,
        ImageFiles = 1 << 2,

        /* UxlTemplate */
        HasCondition = 1 << 4,
        IsDefault = 1 << 5,
    }
}