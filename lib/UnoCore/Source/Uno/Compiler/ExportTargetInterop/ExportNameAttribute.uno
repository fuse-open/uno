namespace Uno.Compiler.ExportTargetInterop
{
    [Obsolete("Legacy from the old JavaScript backend")]
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ExportNameAttribute : Attribute
    {
        public readonly string Name;

        public ExportNameAttribute(string name)
        {
            Name = name;
        }
    }
}
