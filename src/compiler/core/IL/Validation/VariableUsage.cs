using System;

namespace Uno.Compiler.Core.IL.Validation
{
    [Flags]
    enum VariableUsage
    {
        Loaded = 1 << 0,
        Stored = 1 << 1,
    }
}