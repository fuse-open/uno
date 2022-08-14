using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.EventArgs")]
    public class EventArgs
    {
        public static readonly EventArgs Empty = new EventArgs();
    }
}
