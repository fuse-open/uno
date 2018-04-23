using Uno.Collections;
using Uno.IO;
using Uno.Text;
using Uno;

namespace Uno.Data.Xml
{
    public class XmlDocumentElement : XmlLinkedNode
    {
        public XmlDocumentElement()
        {
            NodeType = XmlNodeType.Document;
        }

        protected override XmlLinkedNode CreateNewInstance()
        {
            return new XmlDocumentElement();
        }

        protected override string GetTextContent()
        {
            foreach (var child in Children)
            {
                if (child.NodeType != XmlNodeType.Declaration && child.NodeType != XmlNodeType.ProcessingInstruction)
                    return child.GetTextContent();
            }
            return null;
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            foreach (var child in Children)
            {
                result.Append(child.ToString());
            }
            return result.ToString();
        }
    }
}
