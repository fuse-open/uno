using System;

namespace Uno.Compiler.API.Backends
{
    [Flags]
    public enum BackendOptions
    {
        ExportFiles = 1 << 0,
        ExportMergedBlob = 1 << 1,
        ExportDontExports = 1 << 2,
        AllowInvalidCode = 1 << 3
    }
}