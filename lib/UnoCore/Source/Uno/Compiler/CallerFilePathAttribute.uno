using Uno.Compiler.ExportTargetInterop;

namespace Uno.Compiler
{
    [extern(DOTNET) DotNetType("System.Runtime.CompilerServices.CallerFilePathAttribute")]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class CallerFilePathAttribute : Attribute
    {
    }
}
