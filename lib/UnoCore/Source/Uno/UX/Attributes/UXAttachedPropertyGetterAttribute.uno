using Uno;

namespace Uno.UX
{
    public sealed class UXAttachedPropertyGetterAttribute: Attribute
    {
        public readonly string Name;
        public UXAttachedPropertyGetterAttribute(string name)
        {
            Name = name;
        }
    }
}
