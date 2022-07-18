using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.InvalidOperationException")]
    public class InvalidOperationException : Exception
    {
        public InvalidOperationException()
            : base("Invalid operation")
        {
        }

        public InvalidOperationException(string message)
            : base(message)
        {
        }
    }
}
