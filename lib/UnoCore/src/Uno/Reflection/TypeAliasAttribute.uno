namespace Uno.Reflection
{
    [AttributeUsage(AttributeTargets.Assembly)]
    extern(DOTNET && REFLECTION)
    public class TypeAliasAttribute : Attribute
    {
        public readonly string Alias;
        public readonly string Resolved;

        public TypeAliasAttribute(string alias, string resolved)
        {
            Alias = alias;
            Resolved = resolved;
        }
    }
}
