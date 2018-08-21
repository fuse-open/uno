using Uno.Collections;
using Uno.IO;
using Uno.Text;
using Uno;

namespace Uno.Data.Xml
{
    public class XmlProcessingInstruction : XmlLinkedNode
    {
        public XmlProcessingInstruction()
        {
            NodeType = XmlNodeType.ProcessingInstruction;
        }

        protected override XmlLinkedNode CreateNewInstance()
        {
            return new XmlProcessingInstruction();
        }

        public override string ToString()
        {
            return "<?" + Name + " " + Value.AsString() + "?>";
        }
    }
}
