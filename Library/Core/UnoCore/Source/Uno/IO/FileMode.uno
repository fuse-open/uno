using Uno.Compiler.ExportTargetInterop;

namespace Uno.IO
{
    [extern(DOTNET) DotNetType("System.IO.FileMode")]
    public enum FileMode
    {
        CreateNew = 1,
        Create = 2,
        Open = 3,
        OpenOrCreate = 4,
        Truncate = 5,
        Append = 6,
    }
}