using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.ArgumentNullException")]
    public sealed class ArgumentNullException : ArgumentException
    {
        public ArgumentNullException(string paramName)
            : base("value was null", paramName)
        {
        }
    }
}
