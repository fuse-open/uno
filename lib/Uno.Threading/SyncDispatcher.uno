using Uno;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Threading
{
    internal class SyncDispatcher : IDispatcher
    {
        public void Invoke(Action action)
        {
            action();
        }
    }
}
