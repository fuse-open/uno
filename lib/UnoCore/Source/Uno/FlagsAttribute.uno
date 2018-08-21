using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [AttributeUsage(AttributeTargets.Enum)]
    [extern(DOTNET) DotNetType("System.FlagsAttribute")]
    public sealed class FlagsAttribute : Attribute
    {
    }
}
