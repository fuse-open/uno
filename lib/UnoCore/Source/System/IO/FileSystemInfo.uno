using Uno.Compiler.ExportTargetInterop;
using Uno;

namespace System.IO
{
    [extern(DOTNET) DotNetType]
    extern(DOTNET) public abstract class FileSystemInfo
    {
        public extern FileAttributes Attributes { get; }
        public extern void Refresh();
        public extern bool Exists { get; }
        public extern DateTime LastAccessTimeUtc { get; }
        public extern DateTime LastWriteTimeUtc { get; }
        public extern DateTime CreationTimeUtc { get; }
    }
}
