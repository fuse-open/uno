using Uno;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Threading
{
    public enum FutureState
    {
        Pending,
        Resolved,
        Rejected
    }
}
