using Uno.Compiler.ExportTargetInterop;
using Uno.Collections;
using Uno.IO;
using Uno;

using System.Xml.Linq;

namespace Uno.Data.Xml
{
    [TargetSpecificImplementation]
    class XmlNodeImpl
    {
        [TargetSpecificImplementation]
        public static void AddAttribute(XmlNodeHandle handle, XmlAttributeHandle attrib)
        {
            handle.AddAttribute(attrib);
        }

        [TargetSpecificImplementation]
        public static void AppendChild(XmlNodeHandle handle, XmlNodeHandle child)
        {
            handle.AppendChild(child);
        }

        [TargetSpecificImplementation]
        public static XmlNodeHandle Parse(string xml)
        {
            if defined(CIL)
            {
                var doc = XDocument.Parse(xml);
                var documnetHandle = new XmlNodeHandle();

                if (doc.Declaration != null)
                {
                    documnetHandle.AppendChild(new XmlNodeHandle(doc.Declaration.Version, doc.Declaration.Encoding));
                }

                var child = doc.FirstNode;
                while (child != null)
                {
                    documnetHandle.AppendChild(GenerateXMLTree(child));
                    child = child.NextNode;
                }
                return documnetHandle;
            }
            else
                build_error;
        }

        [TargetSpecificImplementation]
        extern(!CIL) static XmlNodeHandle GenerateXMLTree(TargetSpecificXmlNode xmlNode)
        {
            build_error;
        }

        extern(CIL) static XmlNodeHandle GenerateXMLTree(XNode xmlNode)
        {
            var xmlElement = xmlNode as XElement;
            var xmlName = xmlElement == null ? null : xmlElement.Name;
            string xmlValue = null;

            switch (xmlNode.NodeType)
            {
            case System.Xml.XmlNodeType.CDATA:
                xmlValue = ((XCData)xmlNode).Value;
                break;

            case System.Xml.XmlNodeType.Comment:
                xmlValue = ((XComment)xmlNode).Value;
                break;

            case System.Xml.XmlNodeType.Text:
                xmlValue = ((XText)xmlNode).Value;
                break;

            case System.Xml.XmlNodeType.ProcessingInstruction:
                xmlName = ((XProcessingInstruction)xmlNode).Target;
                xmlValue = ((XProcessingInstruction)xmlNode).Data;
                break;
            }

            var nodeHandle = new XmlNodeHandle(xmlName, xmlValue, xmlNode.NodeType);

            if (xmlElement != null)
            {
                foreach (var attr in xmlElement.Attributes())
                {
                    nodeHandle.AddAttribute(new XmlAttributeHandle(attr));
                }

                var child = xmlElement.FirstNode;
                while (child != null)
                {
                    nodeHandle.AppendChild(GenerateXMLTree(child));
                    child = child.NextNode;
                }
            }

            return nodeHandle;
        }
    }

    [TargetSpecificType]
    struct TargetSpecificXmlNode { }
}
