using Uno.Compiler.ExportTargetInterop;

namespace Uno.Platform
{
    [extern(DOTNET) DotNetType]
    public sealed class ClosingEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
    }
}
