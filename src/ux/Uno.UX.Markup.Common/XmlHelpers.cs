using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Uno.UX.Markup
{
    public class MarkupException : Exception
    {
        public MarkupException(string path, int line, string message)
            : base(message)
        {

        }
    }

    public static class CDataPreProcessor
    {
        class Tag
        {
            public readonly int StartIndex;
            public readonly int StartLength;
            public readonly int EndIndex;
            public readonly int EndLength;

            public Tag(int startIndex, int startLength, int endIndex, int endLength)
            {
                StartIndex = startIndex;
                StartLength = startLength;
                EndIndex = endIndex;
                EndLength = endLength;
            }
        }

        static IEnumerable<Tag> FindTags(string target, string name)
        {
            var startTagMatches = Regex.Match(target, "<" + Regex.Escape(name) + "[^>/]*>", RegexOptions.CultureInvariant);
            var endTagMatches = Regex.Match(target, "</" + Regex.Escape(name) + ">", RegexOptions.CultureInvariant);

            while (startTagMatches.Success && endTagMatches.Success)
            {
                yield return new Tag(
                    startTagMatches.Index,
                    startTagMatches.Length,
                    endTagMatches.Index,
                    endTagMatches.Length);

                startTagMatches = startTagMatches.NextMatch();
                endTagMatches = endTagMatches.NextMatch();
            }
        }

        public static string InsertCData(string xml, params string[] tags)
        {
            var locations = new List<Tag>();
            foreach (var tag in tags)
                locations.AddRange(FindTags(xml, tag));

            var result = new StringBuilder();
            var index = 0;

            foreach (var location in locations)
            {
                var startIndex = location.StartIndex + location.StartLength;
                var endIndex = location.EndIndex;
                var length = endIndex - startIndex;

                result.Append(xml.Substring(index, startIndex - index));
                result.Append("<![CDATA[");
                result.Append(xml.Substring(startIndex, length));
                result.Append("]]>");

                index = endIndex;
            }
            result.Append(xml.Substring(index, xml.Length - index));
            return result.ToString();
        }

    }

    public static class XmlHelpers
    {

        public static XDocument ReadAllXml(this string path, LoadOptions options, bool uxMode)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return ReadAllXml(stream, options, uxMode);
            }
        }

        class XmlEscaper: Uno.Compiler.Frontend.Xml.XmlPreprocessor
        {
            public XmlEscaper(string source): base(source) { }

            protected override string ReEncodeAttribute(string s)
            {
                s = s.Replace("<", "&lt;");
                s = s.Replace(">", "&gt;");
                s = s.Replace("&&", "&amp;&amp;");
                return s;
            }

            protected override bool TransformInnerTextToCDATA
            {
                get
                {
                    return false;
                }
            }
        }

        public static XDocument ReadAllXml(Stream stream, LoadOptions options, bool uxMode)
        {
            using (var reader = new StreamReader(stream))
            {
                var code = reader.ReadToEnd();
                code = CDataPreProcessor.InsertCData(code, "JavaScript");
                code = new XmlEscaper(code).Process();
                using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(code)))
                    return uxMode ? LoadUX(memoryStream, options) : XElement.Load(memoryStream).Document;
            }
        }

        static XDocument LoadUX(Stream data, LoadOptions options)
        {
            var ctx = UxParserContext;
            var readerSettings = new XmlReaderSettings
            {
                IgnoreWhitespace = (options & LoadOptions.PreserveWhitespace) == 0
            };
            using (var reader = XmlReader.Create(data, readerSettings, ctx))
                return XDocument.Load(reader, options);
        }

        public static XmlParserContext UxParserContext
        {
            get
            {
                var mgr = new XmlNamespaceManager(new SimpleNameTable());
                mgr.AddNamespace("ux", Configuration.UXNamespace);
                mgr.AddNamespace("dep", Configuration.DependencyNamespace);
                mgr.AddNamespace("", Configuration.DefaultNamespace);
                return new XmlParserContext(null, mgr, null, XmlSpace.Default);
            }
        }
    }

    class SimpleNameTable : XmlNameTable
    {
        readonly List<string> _cache = new List<string>();

        public override string Add(string array)
        {
            var found = _cache.Find(s =>
                s == array);
            if (found != null)
                return found;
            _cache.Add(array);
            return array;
        }
        public override string Add(char[] array, int offset, int length)
        {
            return Add(new string(array, offset, length));
        }
        public override string Get(string array)
        {
            return _cache.Find(s =>
                s == array);
        }
        public override string Get(char[] array, int offset, int length)
        {
            return Get(new string(array, offset, length));
        }
    }


}
