using System;

namespace Uno.Compiler.API.Domain.Extensions
{
    [Flags]
    public enum CopyFileFlags
    {
        ProcessFile = 1 << 0,
        IsExecutable = 1 << 1,
        IsDirectory = 1 << 2,
        NoOverwrite = 1 << 3,
    }
}