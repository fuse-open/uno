using Uno;

namespace Uno.UX
{
    public sealed class UXDefaultValueAttribute: Attribute
    {
        public readonly string Value;
        public UXDefaultValueAttribute(string value)
        {
            Value = value;
        }
    }
}
