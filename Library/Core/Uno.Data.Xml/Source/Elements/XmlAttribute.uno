using Uno.Collections;
using Uno.IO;
using Uno.Text;
using Uno;

namespace Uno.Data.Xml
{
    public class XmlAttribute : XmlNode
    {
        public XmlAttribute()
        {
            NodeType = XmlNodeType.Attribute;
        }

        public XmlAttribute(string name, string value)
                    : base (name, XmlNodeType.Attribute, new XmlValue(value))
        {
        }

        public XmlAttribute NextSibling { get; internal set; }

        public XmlAttribute PreviousSibling { get; internal set; }

        public XmlAttribute Clone()
        {
            return new XmlAttribute(Name, Value.AsString());
        }

        public override string ToString()
        {
            return Name + "=\"" + Value.AsString() + "\"";
        }
    }
}
