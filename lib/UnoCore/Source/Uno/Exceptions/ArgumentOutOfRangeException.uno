using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.ArgumentOutOfRangeException")]
    public sealed class ArgumentOutOfRangeException : ArgumentException
    {
        public ArgumentOutOfRangeException(string message, string paramName)
            : base(message, paramName)
        {
        }

        public ArgumentOutOfRangeException(string paramName)
            : base("value out of range", paramName)
        {
        }
    }
}
