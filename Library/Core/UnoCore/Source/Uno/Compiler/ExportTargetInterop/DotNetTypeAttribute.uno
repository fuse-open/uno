namespace Uno.Compiler.ExportTargetInterop
{
    [extern(DOTNET) DotNetType]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate)]
    public sealed class DotNetTypeAttribute : Attribute
    {
        public DotNetTypeAttribute(string fullName = null)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class DotNetOverrideAttribute : Attribute
    {
    }
}
