namespace Uno.Compiler.ExportTargetInterop
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class DontExportAttribute : Attribute
    {
    }
}
