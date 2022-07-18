using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.NotSupportedException")]
    public sealed class NotSupportedException : Exception
    {
        public NotSupportedException()
            : base("Method not supported")
        {
        }

        public NotSupportedException(string message)
            : base(message)
        {
        }
    }
}
