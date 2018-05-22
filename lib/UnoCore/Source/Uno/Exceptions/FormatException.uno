using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.FormatException")]
    public sealed class FormatException : Exception
    {
        public FormatException(string message)
            : base(message)
        {
        }
    }
}
