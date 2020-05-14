using System;

namespace Uno.Compiler.API.Domain.AST.Statements
{
    [Flags]
    public enum AstVariableModifier
    {
        Const = 1,
        Extern,
        Mask,
        Shift = 2
    }
}