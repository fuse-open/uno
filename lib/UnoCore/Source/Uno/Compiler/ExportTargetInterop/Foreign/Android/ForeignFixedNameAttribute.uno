namespace Uno.Compiler.ExportTargetInterop.Android
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ForeignFixedNameAttribute : Attribute
    {
        public ForeignFixedNameAttribute() { }
    }
}
