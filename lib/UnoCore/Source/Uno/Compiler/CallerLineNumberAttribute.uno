using Uno.Compiler.ExportTargetInterop;

namespace Uno.Compiler
{
    [extern(DOTNET) DotNetType("System.Runtime.CompilerServices.CallerLineNumberAttribute")]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class CallerLineNumberAttribute : Attribute
    {
    }
}
