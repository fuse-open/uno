using System;

namespace Uno.Compiler.API.Domain.AST.Members
{
    [Flags]
    public enum AstAttributeModifier : byte
    {
        Assembly = 1,
        Module = 2,
        Return = 3,
        HasCondition = 1 << 4,
    }
}