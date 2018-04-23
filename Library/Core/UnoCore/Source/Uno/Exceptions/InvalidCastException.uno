using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.InvalidCastException")]
    public sealed class InvalidCastException : Exception
    {
        public InvalidCastException(string message)
            : base(message)
        {
        }

        public InvalidCastException()
            : this("Invalid cast")
        {
        }
    }
}
