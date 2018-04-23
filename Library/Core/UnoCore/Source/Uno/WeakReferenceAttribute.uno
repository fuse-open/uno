using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Property)]
    public sealed class WeakReferenceAttribute : Attribute
    {
    }
}
