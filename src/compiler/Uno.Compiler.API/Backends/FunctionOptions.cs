using System;

namespace Uno.Compiler.API.Backends
{
    [Flags]
    public enum FunctionOptions
    {
        DecodeEnumOps = 1 << 1,
        DecodeNullOps = 1 << 2,
        DecodeDelegateOps = 1 << 3,
        DecodeSetChains = 1 << 5,
        DecodeSwizzles = 1 << 6,
        MakeNativeCode = 1 << 7,
        Analyze = 1 << 8,
        ClosureConvert = 1 << 9,
        Bytecode = 1 << 10
    }
}
