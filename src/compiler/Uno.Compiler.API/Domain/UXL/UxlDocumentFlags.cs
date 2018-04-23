using System;

namespace Uno.Compiler.API.Domain.UXL
{
    [Flags]
    public enum UxlDocumentFlags : byte
    {
        HasCondition = 1 << 0,
        Usings = 1 << 1,
        Templates = 1 << 2,
        Types = 1 << 3,
        Declarations = 1 << 4,
        Defines = 1 << 5,
        Deprecations = 1 << 6,
    }
}