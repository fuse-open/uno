using Uno.Collections;
using Uno.IO;
using Uno.Text;
using Uno;

namespace Uno.Data.Xml
{
    public class XmlDeclaration : XmlLinkedNode
    {
        public XmlDeclaration()
        {
            NodeType = XmlNodeType.Declaration;
        }

        public XmlDeclaration(string version, XmlEncoding encoding) : this()
        {
            Version = version;
            Encoding = encoding;
        }

        public XmlEncoding Encoding { get; set; }

        public string Version { get; set; }

        protected override XmlLinkedNode CreateNewInstance()
        {
            return new XmlDeclaration(Version, Encoding);
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append("<?xml");

            if (!string.IsNullOrEmpty(Version))
            {
                result.Append(" version=\"" + Version + "\"");
            }

            if (Encoding != XmlEncoding.Auto)
            {
                result.Append(" encoding=\"" + XmlEncodingHelper.ConvertToString(Encoding) + "\"");
            }

            result.Append("?>");
            return result.ToString();
        }
    }
}
