namespace Uno.Compiler.ExportTargetInterop
{
    [extern(DOTNET) DotNetType]
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
    public sealed class TargetSpecificTypeAttribute : Attribute
    {
    }
}
