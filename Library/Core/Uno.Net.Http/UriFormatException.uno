using Uno.Compiler.ExportTargetInterop;

namespace Uno.Net.Http
{
    [DotNetType("System.UriFormatException")]
    public class UriFormatException : Exception
    {
        public UriFormatException(string message)
            : base(message)
        {
        }
    }
}