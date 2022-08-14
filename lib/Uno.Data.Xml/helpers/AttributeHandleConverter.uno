using Uno.Collections;
using Uno;

namespace Uno.Data.Xml
{
    class AttributeHandleConverter
    {
        public static XmlAttribute ConvertToXmlAttribute(XmlAttributeHandle handle)
        {
            return new XmlAttribute(handle.Name, handle.Value);
        }
    }
}
