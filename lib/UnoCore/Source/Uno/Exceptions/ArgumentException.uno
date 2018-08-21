using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.ArgumentException")]
    public class ArgumentException : Exception
    {
        public ArgumentException(string message)
            : base(message)
        {
        }

        public ArgumentException(string message, string paramName)
            : base(paramName + ": " + message)
        {
        }
    }
}
