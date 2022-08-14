using Uno.Collections;
using Uno.IO;
using Uno.Text;
using Uno;

namespace Uno.Data.Xml
{
    public abstract class XmlNode
    {
        protected XmlNode()
        {
            Value = new XmlValue();
        }

        protected XmlNode(string name, XmlNodeType nodeType, XmlValue value)
        {
            NodeType = nodeType;
            Name = name;
            Value = value;
        }

        public XmlNodeType NodeType { get; protected set; }

        public string Name { get; set; }

        public virtual XmlValue Value { get; set; }
    }
}
