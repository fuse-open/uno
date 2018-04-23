using System;

namespace Uno.Compiler.Backends.CPlusPlus
{
    [Flags]
    public enum ParameterFlags
    {
        ObjectByRef = 1 << 0,
        ReturnByRef = 1 << 1,
        ObjectAndReturnByRef = ObjectByRef | ReturnByRef,
    }
}