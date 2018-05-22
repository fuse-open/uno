using Uno.Collections;
using Uno.IO;
using Uno.Text;
using Uno;

namespace Uno.Data.Xml
{
    public abstract class XmlLinkedNode : XmlNode
    {
        protected XmlLinkedNode()
        {
            Children = new XmlLinkedNodeCollection(this);
            Attributes = new XmlAttributeCollection(this);
        }

        public XmlLinkedNode Parent { get; internal set; }

        public XmlLinkedNodeCollection Children { get; private set; }

        public bool HasChildren { get { return Children.HasChildren; } }

        public XmlAttributeCollection Attributes { get; private set; }

        public bool HasAttributes { get { return Attributes.HasChildren; } }

        public XmlLinkedNode NextSibling { get; internal set; }

        public XmlLinkedNode PreviousSibling { get; internal set; }

        public XmlLinkedNode FirstChild { get { return Children.FirstChild; } }

        public XmlLinkedNode LastChild { get { return Children.LastChild; } }

        public string TextContent
        {
            get
            {
                return GetTextContent();
            }

            set
            {
                if (NodeType != XmlNodeType.Element)
                    throw new XmlException("Setting TextContent property is only allowed for XmlElement node");

                Children.Clear();
                Children.Add(new XmlText(new XmlValue(value)));
            }
        }

        public void AppendChild(XmlLinkedNode node)
        {
            Children.Add(node);
        }

        public void PrependChild(XmlLinkedNode node)
        {
            Children.Insert(0, node);
        }

        public void AddAfterSelf(XmlLinkedNode node)
        {
            if (Parent != null)
            {
                if (NextSibling == null)
                {
                    Parent.AppendChild(node);
                }
                else
                {
                    for (int index = 0; index < Parent.Children.Count; index++)
                    {
                        if (Parent.Children[index] == this)
                        {
                            Parent.Children.Insert(index + 1, node);
                            break;
                        }
                    }
                }
            }
        }

        public void AddBeforeSelf(XmlLinkedNode node)
        {
            if (Parent != null)
            {
                if (PreviousSibling == null)
                {
                    Parent.PrependChild(node);
                }
                else
                {
                    for (int index = 0; index < Parent.Children.Count; index++)
                    {
                        if (Parent.Children[index] == this)
                        {
                            Parent.Children.Insert(index, node);
                            break;
                        }
                    }
                }
            }
        }

        public XmlLinkedNode FindByName(string name, bool includeNestedNodes = false)
        {
            var nodes = FindAllByName(name, includeNestedNodes);
            if (nodes.Count > 0)
            {
                return nodes[0];
            }
            return null;
        }

        public IList<XmlLinkedNode> FindAllByName(string name, bool includeNestedNodes = false)
        {
            var res = new List<XmlLinkedNode>();
            RecursiveSearch(res, this, name, includeNestedNodes);
            return res;
        }

        public virtual XmlLinkedNode Clone()
        {
            var newNode = CreateNewInstance();
            newNode.Name = Name;
            newNode.Value = new XmlValue(Value.AsString());

            foreach (var attribute in Attributes)
            {
                newNode.Attributes.Add(attribute.Clone());
            }

            CloneChildren(newNode, this);

            return newNode;
        }

        protected void CloneChildren(XmlLinkedNode newNode, XmlLinkedNode existingNode)
        {
            foreach (var child in existingNode.Children)
            {
                newNode.AppendChild(child.Clone());
            }
        }

        protected virtual string GetTextContent()
        {
            return Value.AsString();
        }

        protected abstract XmlLinkedNode CreateNewInstance();

        private void RecursiveSearch(List<XmlLinkedNode> res, XmlLinkedNode node, string name, bool includeNestedNodes)
        {
            foreach(var child in node.Children)
            {
                if (child.Name == name)
                {
                    res.Add(child);
                }
                if (includeNestedNodes)
                {
                    RecursiveSearch(res, child, name, includeNestedNodes);
                }
            }
        }
    }
}
