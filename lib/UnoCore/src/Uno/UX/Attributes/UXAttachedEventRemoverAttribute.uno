using Uno;

namespace Uno.UX
{
    public sealed class UXAttachedEventRemoverAttribute: Attribute
    {
        public readonly string Name;
        public UXAttachedEventRemoverAttribute(string name)
        {
            Name = name;
        }
    }
}
