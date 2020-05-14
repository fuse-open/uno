using System;

namespace Uno.Compiler.Backends.CPlusPlus
{
    [Flags]
    public enum BodyFlags
    {
        ClassMember = 1 << 0,
        Extension = 1 << 1,
        Inline = 1 << 2,
        ReturnByRef = 1 << 3,
        InlineReturnByRef = Inline | ReturnByRef,
    }
}