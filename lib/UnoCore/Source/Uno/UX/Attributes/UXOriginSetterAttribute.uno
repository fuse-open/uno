using Uno;

namespace Uno.UX
{
    public sealed class UXOriginSetterAttribute: Attribute
    {
        public readonly string OriginSetterName;
        public UXOriginSetterAttribute(string originSetterName)
        {
            OriginSetterName = originSetterName;
        }
    }
}
