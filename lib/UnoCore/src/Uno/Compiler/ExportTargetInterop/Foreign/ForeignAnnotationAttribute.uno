namespace Uno.Compiler.ExportTargetInterop
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    public sealed class ForeignAnnotationAttribute : Attribute
    {
        public readonly Language Language;
        public readonly string[] Annotations;

        public ForeignAnnotationAttribute(Language language, params string[] annotations)
        {
            Language = language;
        }
    }
}
