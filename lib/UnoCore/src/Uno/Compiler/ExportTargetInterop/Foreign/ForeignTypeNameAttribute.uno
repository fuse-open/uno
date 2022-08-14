namespace Uno.Compiler.ExportTargetInterop
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ForeignTypeNameAttribute : Attribute
    {
        public readonly string TypeName;

        public ForeignTypeNameAttribute(string typeName)
        {
            TypeName = typeName;
        }
    }
}
