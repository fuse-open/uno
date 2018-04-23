using System;
using System.Collections.Generic;
using System.Xml;
using Uno.Logging;

namespace Uno.Compiler.Frontend.Xml
{
    public abstract class XmlParserBase : LogObject
    {
        protected readonly SourceFile File;
        protected readonly int StartErrorCount;
        readonly XmlReader Reader;
        readonly string Source;

        protected XmlParserBase(Log log, SourcePackage upk, string filename)
            : base(log)
        {
            File = new SourceFile(upk, filename);
            StartErrorCount = log.ErrorCount;
            Source = XmlPreprocessor.ProcessFile(filename);
            Reader = new XmlTextReader(Source, XmlNodeType.Document, null);
        }

        protected bool GetBool()
        {
            switch (Reader.Value.ToUpperInvariant())
            {
                case "TRUE":
                    return true;
                case "FALSE":
                    return false;
            }

            Log.Error(GetSource(), ErrorCode.E0000, "Invalid bool attribute " + Reader.Value.Quote() + " - expected 'true' or 'false'");
            return false;
        }

        protected int GetInt()
        {
            int result;
            if (int.TryParse(Reader.Value, out result))
                return result;

            Log.Error(GetSource(), ErrorCode.E0000, "Invalid integer attribute " + Reader.Value.Quote());
            return 0;
        }

        protected string GetString()
        {
            return Reader.Value;
        }

        protected SourceValue GetValue()
        {
            return new SourceValue(GetSource(), Reader.Value);
        }

        protected Source GetSource()
        {
            var lineInfo = (IXmlLineInfo)Reader;
            return new Source(File, lineInfo.LineNumber, lineInfo.LinePosition);
        }

        protected string FindRootElement()
        {
            while (Reader.Read())
            {
                switch (Reader.NodeType)
                {
                    case XmlNodeType.XmlDeclaration:
                    case XmlNodeType.Whitespace:
                    case XmlNodeType.Comment:
                        continue;
                }

                break;
            }

            if (Reader.NodeType != XmlNodeType.Element)
                return null;

            return Reader.Name;
        }

        protected void ParseAttributes(Func<string, bool> handler)
        {
            var elmName = Reader.Name;

            for (int i = 0; i < Reader.AttributeCount; i++)
            {
                Reader.MoveToAttribute(i);

                if (!handler(Reader.Name))
                    Log.Error(GetSource(), ErrorCode.E0000, "Unexpected attribute " + Reader.Name.Quote() + " on <" + elmName + "> element");
            }

            Reader.MoveToContent();
        }

        protected void ParseElements(Func<string, bool> handler, bool acceptText = false)
        {
            if (Reader.IsEmptyElement)
                return;

            var elmName = Reader.Name;

            while (true)
            {
                while (Reader.Read())
                {
                    switch (Reader.NodeType)
                    {
                        default:
                            continue;
                        case XmlNodeType.CDATA:
                        case XmlNodeType.Text:
                            if (acceptText)
                                break;
                            continue;
                        case XmlNodeType.Element:
                            break;
                        case XmlNodeType.EndElement:
                            return;
                    }

                    break;
                }

                if (!handler(Reader.Name))
                {
                    Log.Error(GetSource(), ErrorCode.E0000, "Unexpected element " + Reader.Name.Quote() + " in <" + elmName + "> element");
                    Reader.Skip();
                }
            }
        }

        protected void ParseEmptyElement()
        {
            if (Reader.IsEmptyElement)
                return;

            var elmName = Reader.Name;

            while (Reader.Read())
            {
                switch (Reader.NodeType)
                {
                    case XmlNodeType.Comment:
                    case XmlNodeType.Whitespace:
                        continue;

                    case XmlNodeType.EndElement:
                        return;

                    default:
                        Log.Error(GetSource(), ErrorCode.E0000, "Unexpected content in <" + elmName + "> element (" + Reader.NodeType + ")");
                        Reader.Skip();
                        return;
                }
            }
        }

        bool ParseTextElementInternal(string elmName, out Source src, out string text)
        {
            src = null;
            text = null;

            if (Reader.IsEmptyElement)
                return false;

            while (Reader.Read())
            {
                switch (Reader.NodeType)
                {
                    case XmlNodeType.Comment:
                    case XmlNodeType.Whitespace:
                        continue;

                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                        if (!string.IsNullOrEmpty(text))
                            Log.Error(GetSource(), ErrorCode.E0000, "Expected text content inside <" + elmName + "> element (Unexpected " + Reader.NodeType + ")");
                        else
                        {
                            src = GetSource();
                            text = Reader.Value;
                        }
                        continue;

                    case XmlNodeType.EndElement:
                        break;

                    default:
                        src = GetSource();
                        text = "<error>";
                        Log.Error(src, ErrorCode.E0000, "Expected text content inside <" + elmName + "> element (Unexpected " + Reader.NodeType + ")");
                        Reader.Skip();
                        return true;
                }

                break;
            }

            return !string.IsNullOrEmpty(text);
        }

        protected bool ParseTextElement(bool acceptNullOrEmpty, out Source src, out string text)
        {
            var elmName = Reader.Name;
            var result = ParseTextElementInternal(elmName, out src, out text);

            if (!acceptNullOrEmpty && string.IsNullOrEmpty(text))
            {
                src = src ?? GetSource();
                Log.Error(src, ErrorCode.E0000, "Expected non-empty text content inside <" + elmName + ">");
                text = "<error>";
                return true;
            }

            text = SmartTrim(text);
            return result;
        }

        public static string SmartTrim(string str)
        {
            if (str == null)
                return null;

            var ws = new List<char>();

            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];

                if (!char.IsWhiteSpace(c))
                    break;

                if (c == '\n')
                {
                    if (ws.Count == 0)
                        continue;

                    break;
                }

                ws.Add(c);
            }

            if (ws.Count > 0)
            {
                var indent = new string(ws.ToArray());
                str = str.Replace("\n" + indent, "\n");
            }

            return str.Trim();
        }
    }
}
