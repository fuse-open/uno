namespace Uno.Compiler.ExportTargetInterop
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class NativeClassAttribute : Attribute
    {
        public string Language { get; private set; }
        public string ClassName { get; private set; }

        public NativeClassAttribute(string language, string className)
        {
            Language = language;
            ClassName = className;
        }
    }
}
