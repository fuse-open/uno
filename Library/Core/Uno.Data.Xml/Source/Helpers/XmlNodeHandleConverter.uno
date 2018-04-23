using Uno.Collections;
using Uno;

namespace Uno.Data.Xml
{
    class XmlNodeHandleConverter
    {
        public static void ConvertToXmlNodeTree(XmlLinkedNode xmlNode, XmlNodeHandle handle)
        {
            if (handle.NextSibling != null)
            {
                ConvertToXmlNodeTree(xmlNode, handle.NextSibling);
            }

            var newNode = ConvertToXmlLinkedNode(handle);
            if (newNode != null)
            {
                xmlNode.PrependChild(newNode);

                foreach (var attributeHandle in handle.Attributes)
                {
                    newNode.Attributes.Add(AttributeHandleConverter.ConvertToXmlAttribute(attributeHandle));
                }

                if (handle.FirstChild != null)
                {
                    ConvertToXmlNodeTree(newNode, handle.FirstChild);
                }
            }
        }

        private static XmlLinkedNode ConvertToXmlLinkedNode(XmlNodeHandle handle)
        {
            XmlLinkedNode xmlNode;
            switch (handle.NodeType)
            {
                case (int)XmlNodeType.Document:
                    xmlNode = new XmlDocumentElement();
                    break;
                case (int)XmlNodeType.Element:
                    xmlNode = new XmlElement();
                    break;
                case (int)XmlNodeType.Comment:
                    xmlNode = new XmlComment();
                    break;
                case (int)XmlNodeType.CharacterData:
                    xmlNode = new XmlCharacterData(handle.Value);
                    break;
                case (int)XmlNodeType.Text:
                    if (handle.Value == null || handle.Value.Trim().Length == 0)
                    {
                        return null;
                    }
                    xmlNode = new XmlText();
                    break;
                case (int)XmlNodeType.Declaration:
                    xmlNode = new XmlDeclaration(handle.Version, XmlEncodingHelper.ConvertFromString(handle.Encoding));
                    break;
                case (int)XmlNodeType.ProcessingInstruction:
                    xmlNode = new XmlProcessingInstruction();
                    break;
                default:
                    return null;
            }

            xmlNode.Name = handle.Name;
            if (handle.Value != null)
            {
                xmlNode.Value = new XmlValue(handle.Value);
            }

            return xmlNode;
        }
    }
}
