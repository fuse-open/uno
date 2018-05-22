using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.ObjectDisposedException")]
    public sealed class ObjectDisposedException : Exception
    {
        public ObjectDisposedException(string objectName)
            : base("Attempt to access disposed object: " + objectName)
        {
        }
    }
}
