using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [AttributeUsage(AttributeTargets.Class)]
    [extern(DOTNET) DotNetType("System.AttributeUsageAttribute")]
    public sealed class AttributeUsageAttribute : Attribute
    {
        public AttributeUsageAttribute(AttributeTargets validOn)
        {
        }
    }
}
