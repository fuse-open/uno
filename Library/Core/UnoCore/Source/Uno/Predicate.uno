using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.Predicate`1")]
    public delegate bool Predicate<T>(T arg);
}
