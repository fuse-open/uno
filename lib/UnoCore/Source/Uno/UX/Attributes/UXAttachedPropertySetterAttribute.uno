using Uno;

namespace Uno.UX
{
    public sealed class UXAttachedPropertySetterAttribute: Attribute
    {
        public readonly string Name;
        public UXAttachedPropertySetterAttribute(string name)
        {
            Name = name;
        }
    }

    public sealed class UXAttachedPropertyStyleSetterAttribute: Attribute
    {
        public readonly string Name;
        public UXAttachedPropertyStyleSetterAttribute(string name)
        {
            Name = name;
        }
    }
}
