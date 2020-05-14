using System;

namespace Uno.Compiler.Backends.CPlusPlus
{
    [Flags]
    public enum TypeFlags : byte
    {
        Parameter = 1,
        Return = 2,
        This = 3,

        TypeMask = 0xf,

        ByRef = 1 << 4,
        Declaration = 1 << 5,
        Inline = 1 << 6,

        ParameterByRef = Parameter | ByRef,
        ReturnByRef = Return | ByRef,
        ReturnInline = Return | Inline,
        ThisByRef = This | ByRef,
        ThisByRefDeclaration = This | ByRef | Declaration,
        ThisDeclaration = This | Declaration,
        ThisInline = This | Inline,
        ThisInlineDeclaration = This | Inline | Declaration,
    }
}