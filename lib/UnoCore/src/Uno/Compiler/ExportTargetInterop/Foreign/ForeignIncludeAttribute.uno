namespace Uno.Compiler.ExportTargetInterop
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor)]
    public sealed class ForeignIncludeAttribute : Attribute
    {
        public readonly Language Language;
        public readonly string[] Includes;

        public ForeignIncludeAttribute(Language language, params string[] includes)
        {
            Language = language;
            Includes = includes;
        }
    }
}
