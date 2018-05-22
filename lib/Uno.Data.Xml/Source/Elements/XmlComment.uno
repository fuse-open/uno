using Uno.Collections;
using Uno.IO;
using Uno.Text;
using Uno;

namespace Uno.Data.Xml
{
    public class XmlComment : XmlLinkedNode
    {
        public XmlComment()
        {
            NodeType = XmlNodeType.Comment;
        }

        protected override XmlLinkedNode CreateNewInstance()
        {
            return new XmlComment();
        }

        public override string ToString()
        {
            return "<!--" + Value.AsString() + "-->";
        }
    }
}
