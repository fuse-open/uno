namespace Uno.Compiler.ExportTargetInterop
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class GlslIntrinsicAttribute : Attribute
    {
        public readonly string Name;

        public GlslIntrinsicAttribute(string name)
        {
            Name = name;
        }
    }
}
