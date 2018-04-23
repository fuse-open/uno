using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.OverflowException")]
    public sealed class OverflowException : Exception
    {
        public OverflowException(string message)
            : base(message)
        {
        }
    }
}
