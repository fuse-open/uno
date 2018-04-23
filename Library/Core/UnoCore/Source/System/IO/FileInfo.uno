using Uno.Compiler.ExportTargetInterop;

namespace System.IO
{
    [extern(DOTNET) DotNetType]
    extern(DOTNET) public sealed class FileInfo : FileSystemInfo
    {
        public extern FileInfo(string path);
        public extern long Length { get; }
    }
}
