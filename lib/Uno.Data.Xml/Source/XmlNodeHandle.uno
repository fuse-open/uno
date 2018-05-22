using Uno.Compiler.ExportTargetInterop;
using Uno.Collections;
using Uno.IO;
using Uno;

using System.Xml.Linq;

namespace Uno.Data.Xml
{
    extern(CIL) class XmlNodeTypeConverter
    {
        public static int Get(System.Xml.XmlNodeType type)
        {
            switch (type)
            {
                case System.Xml.XmlNodeType.Document:
                    return 0;
                case System.Xml.XmlNodeType.Text:
                    return 1;
                case System.Xml.XmlNodeType.CDATA:
                    return 2;
                case System.Xml.XmlNodeType.Element:
                    return 3;
                case System.Xml.XmlNodeType.XmlDeclaration:
                    return 4;
                case System.Xml.XmlNodeType.Comment:
                    return 5;
                case System.Xml.XmlNodeType.ProcessingInstruction:
                    return 6;
                default:
                    return -1;
            }
        }
    }

    [TargetSpecificImplementation]
    internal class XmlNodeHandle
    {
        public string Name { get; private set; }
        public string Value { get; set; }
        public int NodeType { get; set; } // -1 - unknown
        public string Encoding { get; set; }
        public string Version { get; set; }

        public XmlNodeHandle Parent { get; set; }
        public XmlNodeHandle NextSibling { get; set; }
        public XmlNodeHandle PrevSibling { get; set; }

        public XmlNodeHandle FirstChild
        {
            get { return (Children.Count > 0) ? Children[0] : null; }
        }

        public XmlNodeHandle LastChild
        {
            get { return (Children.Count > 0) ? Children.Last() : null; }
        }

        List<XmlAttributeHandle> _attributes = null;
        public List<XmlAttributeHandle> Attributes
        {
            get
            {
                return (_attributes == null) ? _attributes = new List<XmlAttributeHandle>() : _attributes;
            }
        }

        List<XmlNodeHandle> _children = null;
        public List<XmlNodeHandle> Children
        {
            get
            {
                return (_children == null) ? _children = new List<XmlNodeHandle>() : _children;
            }
        }

        public XmlNodeHandle[] ChildrenHandles
        {
            get { return (_children != null) ? _children.ToArray() : null; }
        }

        public XmlAttributeHandle[] AttributeHandles
        {
            get { return (_attributes != null) ? _attributes.ToArray() : null; }
        }

        //root node
        public XmlNodeHandle()
        {
            Parent = null;
            NextSibling = null;
            PrevSibling = null;
        }

        public extern(!CIL) XmlNodeHandle(string name, string value, int nodeType)
        {
            Name = name;
            Value = string.IsNullOrEmpty(value) ? null : value;
            NodeType = nodeType;
            Parent = null;
            NextSibling = null;
            PrevSibling = null;
        }

        public extern(CIL) XmlNodeHandle(XName name, string value, System.Xml.XmlNodeType nodeType)
        {
            Name = name != null ? name.LocalName : null;
            Value = value;
            NodeType = XmlNodeTypeConverter.Get(nodeType);
            Parent = null;
            NextSibling = null;
            PrevSibling = null;
        }

        public XmlNodeHandle(string xmlVersion, string xmlEncoding)
        {
            Name = "xml";
            Encoding = xmlEncoding;
            Version = xmlVersion;
            NodeType = (int)XmlNodeType.Declaration;
            Parent = null;
            NextSibling = null;
            PrevSibling = null;
        }

        public void AppendChild(XmlNodeHandle child)
        {
            if (child == null)
                return;

            var lastChild = LastChild;
            child.Parent = this;
            child.PrevSibling = lastChild;
            if (lastChild != null)
                lastChild.NextSibling = child;

            child.NextSibling = null;
            Children.Add(child);
        }

        public void AddAttribute(XmlAttributeHandle attribute)
        {
            Attributes.Add(attribute);
        }
    }
}
