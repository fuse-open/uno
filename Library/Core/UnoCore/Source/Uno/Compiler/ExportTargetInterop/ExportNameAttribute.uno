namespace Uno.Compiler.ExportTargetInterop
{
    [extern(DOTNET) DotNetType]
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
