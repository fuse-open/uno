using Uno.Collections;
using Uno.IO;
using Uno.Text;
using Uno;

namespace Uno.Data.Xml
{
    public class XmlCharacterData : XmlLinkedNode
    {
        public XmlCharacterData()
        {
            NodeType = XmlNodeType.CharacterData;
        }

        public XmlCharacterData(string data) : this()
        {
            Data = data;
        }

        public string Data { get; set; }

        public int Length { get { return Data.Length; } }

        protected override XmlLinkedNode CreateNewInstance()
        {
            return new XmlCharacterData(Data);
        }

        public override string ToString()
        {
            return "<![CDATA[" + Data + "]]>";
        }
    }
}
