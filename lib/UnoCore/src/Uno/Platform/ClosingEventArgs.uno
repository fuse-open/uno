using Uno.Compiler.ExportTargetInterop;

namespace Uno.Platform
{
    public sealed class ClosingEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
    }
}
