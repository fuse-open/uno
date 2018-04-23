using Uno.Compiler.ExportTargetInterop;
using Uno.IO;

namespace System
{
    [extern(DOTNET) DotNetType]
    public static extern(DOTNET) class Console
    {
        public static extern TextWriter Error { get; }
        public static extern TextWriter Out { get; }
        public static extern void WriteLine(string value);
    }
}
