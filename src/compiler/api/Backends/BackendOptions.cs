using System;

namespace Uno.Compiler.API.Backends
{
    [Flags]
    public enum BackendOptions
    {
        AllowInvalidCode = 1 << 0,
        ExportDontExports = 1 << 1,
    }
}