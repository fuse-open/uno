using Uno;

namespace Uno.Data.Xml
{
    public enum XmlNodeType
    {
        Document,
        Text,
        CharacterData,
        Element,
        Declaration,
        Comment,
        ProcessingInstruction,
        Attribute
    }

    public enum XmlEncoding
    {
        Auto,
        Utf8,
        Utf16_le,
        Utf16_be,
        Utf16,
        Utf32_le,
        Utf32_be,
        Utf32,
        Wchar,
        Latin1
    }

    [FlagsAttribute]
    public enum ParseOptions : uint
    {
        IncludeProcessingInstruction = 0x0001,
        IncludeDeclaration = 0x0100,
        IncludeComment = 0x0002,
        IncludeCharacterData = 0x0004
    }

    public enum XmlValueType
    {
        Bool,
        Int,
        Long,
        Float,
        Double,
        String
    }
}