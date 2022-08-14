using Uno.Compiler.ExportTargetInterop;

namespace Uno.Compiler
{
    [extern(DOTNET) DotNetType("System.Runtime.CompilerServices.CallerMemberNameAttribute")]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class CallerMemberNameAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class CallerPackageNameAttribute : Attribute
    {
    }
}
