using Uno;

namespace Uno.UX
{
    public sealed class UXAttachedEventAdderAttribute: Attribute
    {
        public readonly string Name;
        public UXAttachedEventAdderAttribute(string name)
        {
            Name = name;
        }
    }
}
