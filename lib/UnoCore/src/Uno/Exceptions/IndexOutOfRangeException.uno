using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.IndexOutOfRangeException")]
    public sealed class IndexOutOfRangeException : Exception
    {
        public IndexOutOfRangeException()
            : base("Index out of range")
        {
        }
    }
}
