using Uno.Collections;
using Uno;

namespace Uno.Data.Xml
{
    class ValidationHelper
    {
        public static void ValidateAttributeAdding(XmlLinkedNode owner, XmlAttribute item)
        {
            if (owner.NodeType != XmlNodeType.Element)
            {
                throw new XmlException("Attributes can be added to xml elements only");
            }

            foreach (var existingAttr in owner.Attributes)
            {
                if (existingAttr.Name == item.Name)
                {
                    throw new XmlException("Attribute with the same name already exists");
                }
            }
        }

        public static void ValidateNodeAdding(XmlLinkedNode owner, XmlLinkedNode item)
        {
            if (owner.NodeType != XmlNodeType.Element && owner.NodeType != XmlNodeType.Document)
            {
                throw new XmlException("Children can be added to the root node or to xml elements");
            }

            if (item.NodeType == XmlNodeType.Declaration && owner.NodeType != XmlNodeType.Document)
            {
                throw new XmlException("Declaration can be added to the root node only");
            }

            if (owner.NodeType == XmlNodeType.Document)
            {
                if (item.NodeType == XmlNodeType.Text || item.NodeType == XmlNodeType.CharacterData || item.NodeType == XmlNodeType.Comment)
                {
                    throw new XmlException("This type of node can't be added to the root element");
                }

                if (item.NodeType == XmlNodeType.Declaration && IsChildAlreadyExists(owner, XmlNodeType.Declaration))
                {
                    throw new XmlException("Only one declaration is possible");
                }

                if (item.NodeType == XmlNodeType.Element && IsChildAlreadyExists(owner, XmlNodeType.Element))
                {
                    throw new XmlException("Only one xml element can be added to the root");
                }
            }
        }

        private static bool IsChildAlreadyExists(XmlLinkedNode owner, XmlNodeType nodeType)
        {
            foreach (var child in owner.Children)
            {
                if (child.NodeType == nodeType)
                    return true;
            }
            return false;
        }
    }
}
