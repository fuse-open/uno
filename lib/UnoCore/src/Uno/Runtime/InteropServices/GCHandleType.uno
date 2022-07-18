using Uno.Compiler.ExportTargetInterop;

namespace Uno.Runtime.InteropServices
{
    [extern(DOTNET) DotNetType("System.Runtime.InteropServices.GCHandleType")]
    public enum GCHandleType
    {
        // Weak = 0,
        // WeakTrackResurrection = 1,
        Normal = 2,
        Pinned = 3,
    }
}
