using Uno.Compiler.ExportTargetInterop;

namespace Uno.IO
{
    [extern(DOTNET) DotNetType("System.IO.SeekOrigin")]
    public enum SeekOrigin
    {
        Begin,
        Current,
        End,
    }
}
