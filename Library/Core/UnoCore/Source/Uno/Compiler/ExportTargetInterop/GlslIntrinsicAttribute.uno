namespace Uno.Compiler.ExportTargetInterop
{
    [extern(DOTNET) DotNetType]
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
