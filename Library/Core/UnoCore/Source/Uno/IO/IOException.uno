using Uno.Compiler.ExportTargetInterop;

namespace Uno.IO
{
    [extern(DOTNET) DotNetType("System.IO.IOException")]
    public class IOException : Exception
    {
        public IOException(string message)
            : base(message)
        {
        }
    }

    [extern(DOTNET) DotNetType("System.IO.EndOfStreamException")]
    public class EndOfStreamException : IOException
    {
        public EndOfStreamException()
            : base("Reached end of stream")
        {
        }
    }

    [extern(DOTNET) DotNetType("System.IO.FileNotFoundException")]
    public class FileNotFoundException : IOException
    {
        public string FileName { get; private set; }

        public FileNotFoundException(string message, string filename)
            : base(message)
        {
            FileName = filename;
        }
    }
}
