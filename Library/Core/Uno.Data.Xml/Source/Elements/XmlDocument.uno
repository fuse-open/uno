using Uno.Collections;
using Uno.IO;
using Uno.Text;
using Uno;

namespace Uno.Data.Xml
{
    public class XmlDocument
    {
        public XmlDocument()
        {
            DocumentElement = new XmlDocumentElement();
        }

        public XmlDocumentElement DocumentElement { get; private set; }

        /*
        public static XmlDocument Load(Stream stream)
        {
            //Convert Stream to string and call Load for string
            throw new NotSupportedException();
        }

        public static XmlDocument Load(Stream stream, XmlEncoding encoding)
        {
            //Convert Stream to string and call Load for string
            throw new NotSupportedException();
        }
        */

        public static XmlDocument Load(string xmlString)
        {
            return Load(xmlString, ParseOptions.IncludeCharacterData);
        }

        public static XmlDocument Load(string xmlString, ParseOptions parseOptions)
        {
            //make sure that XML declaration allowed only at the start of the document
            if (xmlString.IndexOf("<?xml") > 0)
            {
                throw new XmlException("XML declaration allowed only at the start of the document");
            }

            //make sure that default parse options applied
            parseOptions = parseOptions | ParseOptions.IncludeCharacterData;

            var rootHandle = XmlNodeImpl.Parse(xmlString);
            var xmlDocument = new XmlDocument();

            if (rootHandle != null && rootHandle.FirstChild != null)
            {
                XmlNodeHandleConverter.ConvertToXmlNodeTree(xmlDocument.DocumentElement, rootHandle.FirstChild);
            }

            ApplyParseOptions(xmlDocument.DocumentElement, parseOptions);

            return xmlDocument;
        }

        /*
        public void Save(Stream stream)
        {
            throw new NotSupportedException();
        }
        */

        public void Save(out string xmlString)
        {
            xmlString = DocumentElement.ToString();
        }

        public override string ToString()
        {
            return DocumentElement.ToString();
        }

        private static void ApplyParseOptions(XmlLinkedNode node, ParseOptions parseOptions)
        {
            var includePI = (parseOptions & ParseOptions.IncludeProcessingInstruction) == ParseOptions.IncludeProcessingInstruction;
            var includeCD = (parseOptions & ParseOptions.IncludeCharacterData) == ParseOptions.IncludeCharacterData;
            var includeDeclaration = (parseOptions & ParseOptions.IncludeDeclaration) == ParseOptions.IncludeDeclaration;
            var includeComment = (parseOptions & ParseOptions.IncludeComment) == ParseOptions.IncludeComment;

            for (var i = 0; i < node.Children.Count; i++)
            {
                var childNode = node.Children[i];
                if ((childNode.NodeType == XmlNodeType.ProcessingInstruction && !includePI) ||
                    (childNode.NodeType == XmlNodeType.CharacterData && !includeCD) ||
                    (childNode.NodeType == XmlNodeType.Declaration && !includeDeclaration) ||
                    (childNode.NodeType == XmlNodeType.Comment && !includeComment))
                {
                    node.Children.Remove(childNode);
                    i--;
                }
                else
                {
                    ApplyParseOptions(childNode, parseOptions);
                }
            }
        }
    }
}
