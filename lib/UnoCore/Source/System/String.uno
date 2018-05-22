using Uno.Compiler.ExportTargetInterop;

namespace System
{
    [extern(DOTNET) DotNetType]
    public extern(DOTNET) class String
    {
        public static extern string Format(string format, object arg0);
    }
}
