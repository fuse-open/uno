using System;

namespace Uno.Compiler.API.Backends
{
    [Flags]
    public enum TypeOptions
    {
        MakeUniqueNames = 1 << 0,
        IgnoreAttributes = 1 << 1,
        IgnoreProtection = 1 << 2,
        FlattenConstructors = 1 << 3,
        FlattenEvents = 1 << 4,
        FlattenProperties = 1 << 5,
        FlattenOperators = 1 << 6,
        FlattenCasts = 1 << 7
    }
}