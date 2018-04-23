using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.Comparison`1")]
    public delegate int Comparison<T>(T a, T b);
}
