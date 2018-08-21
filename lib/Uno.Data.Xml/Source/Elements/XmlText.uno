using Uno.Collections;
using Uno.IO;
using Uno.Text;
using Uno;

namespace Uno.Data.Xml
{
    public class XmlText : XmlLinkedNode
    {
        public XmlText()
        {
            NodeType = XmlNodeType.Text;
        }

        public XmlText(XmlValue value) : this()
        {
            Value = value;
        }

        protected override XmlLinkedNode CreateNewInstance()
        {
            return new XmlText();
        }

        public override string ToString()
        {
            return Value.AsString();
        }
    }
}
