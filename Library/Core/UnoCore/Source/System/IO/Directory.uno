using Uno.Compiler.ExportTargetInterop;
using Uno.Collections;
using Uno.IO;
using Uno;

namespace System.IO
{
    [extern(DOTNET) DotNetType]
    public static extern(DOTNET) class Directory
    {
        public static extern DirectoryInfo CreateDirectory(string dirName);
    }
}
