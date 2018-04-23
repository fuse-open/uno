using Uno;

namespace Uno.UX
{
    public sealed class UXValueBindingAliasAttribute: Attribute
    {
        public readonly string Alias;

        public UXValueBindingAliasAttribute(string alias)
        {
            Alias = alias;
        }
    }
}
