using Uno;

namespace Uno.UX
{
    public sealed class UXAutoGenericAttribute: Attribute
    {
        public readonly string Alias;
        public readonly string Property;

        public UXAutoGenericAttribute(string alias, string property)
        {
            Alias = alias;
            Property = property;
        }
    }
}
