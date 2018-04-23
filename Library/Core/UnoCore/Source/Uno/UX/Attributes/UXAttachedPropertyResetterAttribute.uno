using Uno;

namespace Uno.UX
{
    public sealed class UXAttachedPropertyResetterAttribute: Attribute
    {
        public readonly string Name;
        public UXAttachedPropertyResetterAttribute(string name)
        {
            Name = name;
        }
    }
}
