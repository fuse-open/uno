namespace Uno.Compiler.ExportTargetInterop
{
    [extern(DOTNET) DotNetType]
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate)]
    public sealed class TargetSpecificImplementationAttribute : Attribute
    {
    }
}
