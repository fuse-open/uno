using Uno.Compiler.ExportTargetInterop;

namespace Uno.Threading
{
    [DotNetType("System.Threading.AutoResetEvent")]
    public sealed class AutoResetEvent : EventWaitHandle
    {
        public AutoResetEvent(bool initialState) : base(initialState, EventResetMode.AutoReset)
        {
        }
    }
}
