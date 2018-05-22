using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.NotImplementedException")]
    public sealed class NotImplementedException : Exception
    {
        public NotImplementedException()
            : base("Not implemented")
        {
        }
    }
}
