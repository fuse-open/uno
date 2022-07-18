using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.NullReferenceException")]
    public sealed class NullReferenceException : Exception
    {
        public NullReferenceException()
            : base("Object reference was null")
        {
        }
    }
}
