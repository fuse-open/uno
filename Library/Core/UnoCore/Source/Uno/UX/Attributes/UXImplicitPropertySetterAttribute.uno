
namespace Uno.UX
{
    public class UXImplicitPropertySetterAttribute: Attribute
    {
        public readonly string Type;
        public UXImplicitPropertySetterAttribute(string type)
        {
            Type = type;
        }
    }
}