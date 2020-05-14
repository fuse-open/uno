using System;

namespace Uno.Compiler.API.Domain.AST
{
    [Flags]
    public enum AstSerializationFlags
    {
        OptimizeSources = 1 << 0
    }
}