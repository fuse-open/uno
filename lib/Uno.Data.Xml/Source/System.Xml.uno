using Uno.Compiler.ExportTargetInterop;
using Uno.Collections;

namespace System.Xml
{
    [DotNetType]
    extern(CIL) enum XmlNodeType
    {
        None = 0,
        Element = 1,
        Attribute = 2,
        Text = 3,
        CDATA = 4,
        EntityReference = 5,
        Entity = 6,
        ProcessingInstruction = 7,
        Comment = 8,
        Document = 9,
        DocumentType = 10,
        DocumentFragment = 11,
        Notation = 12,
        Whitespace = 13,
        SignificantWhitespace = 14,
        EndElement = 15,
        EndEntity = 16,
        XmlDeclaration = 17
    }
}

namespace System.Xml.Linq
{
    [DotNetType]
    extern(CIL) sealed class XName
    {
        public extern static implicit operator XName(string expandedName);
        public extern string LocalName { get; }
    }

    [DotNetType]
    extern(CIL) abstract class XNode
    {
        public extern abstract XmlNodeType NodeType { get; }
        public extern XNode NextNode { get; }
    }

    [DotNetType]
    extern(CIL) class XComment : XNode
    {
        public extern override XmlNodeType NodeType { get; }
        public extern string Value { get; set; }
    }

    [DotNetType]
    extern(CIL) class XProcessingInstruction : XNode
    {
        public extern override XmlNodeType NodeType { get; }
        public extern string Target { get; set; }
        public extern string Data { get; set; }
    }

    [DotNetType]
    extern(CIL) abstract class XContainer : XNode
    {
    }

    [DotNetType]
    extern(CIL) class XAttribute
    {
        public extern XName Name { get; }
        public extern string Value { get; set; }
    }

    [DotNetType]
    extern(CIL) sealed class XElement : XContainer
    {
        public extern override XmlNodeType NodeType { get; }
        public extern XName Name { get; set; }
        public extern XNode FirstNode { get; }
        public extern IEnumerable<XAttribute> Attributes();
    }

    [DotNetType]
    extern(CIL) abstract class XText : XNode
    {
        public extern string Value { get; set; }
    }

    [DotNetType]
    extern(CIL) sealed class XCData : XText
    {
        public extern override XmlNodeType NodeType { get; }
    }

    [DotNetType]
    extern(CIL) sealed class XDeclaration
    {
        public extern string Version { get; set; }
        public extern string Encoding { get; set; }
    }

    [DotNetType]
    extern(CIL) sealed class XDocument
    {
        public extern static XDocument Parse(string text);
        public extern XDeclaration Declaration { get; set; }
        public extern XNode FirstNode { get; }
    }
}
