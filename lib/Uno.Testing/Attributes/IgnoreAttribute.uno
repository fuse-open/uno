namespace Uno.Testing
{
    [AttributeUsage(AttributeTargets.Method)]
    public class IgnoreAttribute : Attribute
    {
        public readonly string Reason;
        public readonly string Condition;

        public IgnoreAttribute(string reason="", string condition="")
        {
            Reason = reason;
            Condition = condition;
        }
    }
}
