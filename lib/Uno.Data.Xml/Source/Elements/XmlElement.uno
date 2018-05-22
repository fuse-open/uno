using Uno.Collections;
using Uno.IO;
using Uno.Text;
using Uno;

namespace Uno.Data.Xml
{
    public class XmlElement : XmlLinkedNode
    {
        public XmlElement()
        {
            NodeType = XmlNodeType.Element;
        }

        protected override XmlLinkedNode CreateNewInstance()
        {
            return new XmlElement();
        }

        protected override string GetTextContent()
        {
            var strBuilder = new StringBuilder();
            var needWhitespaceBetweenContents = false;
            foreach (var child in Children)
            {
                if (child.NodeType != XmlNodeType.Comment && child.NodeType != XmlNodeType.ProcessingInstruction)
                {
                    var childTextContent = child.GetTextContent();
                    if (!string.IsNullOrEmpty(childTextContent))
                    {
                        if (childTextContent[0] != ' ' && needWhitespaceBetweenContents)
                        {
                            strBuilder.Append(" ");
                        }
                        strBuilder.Append(childTextContent);
                        needWhitespaceBetweenContents = childTextContent[childTextContent.Length - 1] != ' ';
                    }
                }
            }
            return strBuilder.ToString();
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append("<" + Name);

            foreach (var a in Attributes)
            {
                result.Append(" " + a.ToString());
            }

            if (HasChildren)
            {
                result.Append(">" + ChildrenToString() + "</" + Name + ">");
            }
            else
            {
                result.Append("/>");
            }
            return result.ToString();
        }

        public override XmlValue Value
        {
            get
            {
                if (Children.Count == 1 && FirstChild.NodeType == XmlNodeType.Text)
                    return FirstChild.Value;
                return base.Value;
            }
            set
            {
                base.Value = value;
            }
        }

        private string ChildrenToString()
        {
            var strBuilder = new StringBuilder();
            foreach (var child in Children)
            {
                strBuilder.Append(child.ToString());
            }
            return strBuilder.ToString();
        }
    }
}
