using System;

namespace Uno.Compiler.API.Domain.UXL
{
    [Flags]
    public enum UxlEntityFlags : byte
    {
        Elements = 1 << 0,
        CopyFiles = 1 << 1,
        ImageFiles = 1 << 2,
    }
}