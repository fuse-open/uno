namespace Uno.Compiler.ExportTargetInterop
{
    [Obsolete("Use the extern(CONDITION) modifier instead")]
    public sealed class ExportConditionAttribute : Attribute
    {
        public readonly string Condition;

        public ExportConditionAttribute(string condition)
        {
            Condition = condition;
        }
    }
}
