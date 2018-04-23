using System;
using System.IO;
using System.Net;
using System.Text;

namespace Uno.Compiler.Frontend.Xml
{
    public class XmlPreprocessor
    {
        public static string ProcessFile(string filename)
        {
            return Process(File.ReadAllText(filename));
        }

        public static string Process(string source)
        {
            return new XmlPreprocessor(source).Process();
        }

        readonly StringBuilder _sb = new StringBuilder();
        readonly string _source;
        int _index;

        protected XmlPreprocessor(string source)
        {
            _source = source;
        }

        protected virtual string ReEncodeAttribute(string s)
        {
            return WebUtility.HtmlEncode(WebUtility.HtmlDecode(s));
        }

        protected virtual bool TransformInnerTextToCDATA
        {
            get { return true; }
        }


        public string Process()
        {
            while (Scan('<'))
            {
                var element = ParseName();

                if (string.IsNullOrEmpty(element))
                    continue;

                // Skip CDATA
                if (element.StartsWith("![CDATA["))
                {
                    Scan("]]>");
                    continue;
                }

                // Skip comment
                if (element.StartsWith("!--"))
                {
                    Scan("-->");
                    continue;
                }

                var start = _index;

                for (; ; )
                {
                    var attr = ParseName();

                    if (string.IsNullOrEmpty(attr) || _index == _source.Length)
                        break;

                    start = _index;
                    SkipWs(ref start);
                    var c = _source[start];

                    if (c != '=')
                    {
                        // Insert ATTRIBUTE="ATTRIBUTE" when a value was missing, if this looks like sane XML
                        if (LooksLikeValidAttribute(attr, c))
                            _sb.Append("=\"" + attr + "\"");

                        ConsumeTo(start);
                        continue;
                    }

                    start++;
                    SkipWs(ref start);
                    ConsumeTo(start);

                    if (_index == _source.Length)
                        break;

                    c = _source[_index];
                    if (c == '"' || c == '\'')
                    {
                        for (var end = _index + 1; end < _source.Length; end++)
                        {
                            var e = _source[end];
                            if (e == c)
                            {
                                // Consume first quote character (c)
                                Consume();

                                // Escape XML entities in quoted value (using implementation-specific encoder)
                                _sb.Append(ReEncodeAttribute(_source.Substring(_index, end - _index)));
                                _sb.Append(e);
                                _index = start = end + 1;
                                break;
                            }

                            // Invalid newline in quoted value, stop parsing attributes
                            if (e == '\n')
                                goto END;
                        }
                    }
                    else if (IsNameChar(c))
                    {
                        var end = _index + 1;
                        while (end < _source.Length && IsNameChar(_source[end]))
                            end++;

                        var next = end;
                        SkipWs(ref next);

                        if (next == _source.Length)
                            continue;

                        // Insert missing quotes, if XML looks sane
                        if (LooksLikeValidAttribute(attr, _source[next]))
                        {
                            _sb.Append('"');
                            _sb.Append(_source, _index, end - _index);
                            _sb.Append('"');
                            _index = start = end;
                        }
                    }
                }

            END:
                if (Scan('>'))
                {
                    var isClosed = false;
                    for (int i = _index - 1; i >= start; i--)
                    {
                        var c = _source[i];
                        if (c == '/')
                        {
                            isClosed = true;
                            break;
                        }

                        if (!char.IsWhiteSpace(c))
                            break;
                    }

                    // No inner text in <Closed /> elements
                    if (isClosed)
                        continue;

                    var end = _index;
                    SkipWs(ref end);

                    if (end == _source.Length || _source[end] == '<')
                        continue;

                    end = FindEnd(element, end);
                    if (end == -1)
                        continue;

                    // Insert CDATA decoration on inner text
                    if (TransformInnerTextToCDATA) _sb.Append("<![CDATA[");
                    _sb.Append(_source, _index, end - _index);
                    if (TransformInnerTextToCDATA) _sb.Append("]]>");
                    _index = end;
                }
            }

            return _sb.ToString();
        }

        bool LooksLikeValidAttribute(string name, char next)
        {
            return !string.IsNullOrEmpty(name) &&
                   (name[0] == '_' || char.IsLetter(name[0])) &&
                   (next == '/' || next == '>' || IsNameChar(next));
        }

        int FindEnd(string element, int start)
        {
            for (; ; )
            {
                var retval = _source.UnquotedIndexOf('<', start);

                if (retval == -1)
                    return -1;

                start = retval + 1;
                SkipWs(ref start);
                if (start == _source.Length || _source[start] != '/')
                    continue;

                start++;
                SkipWs(ref start);

                var end = start;
                while (end < _source.Length && IsNameChar(_source[end]))
                    end++;

                var value = _source.Substring(start, end - start);

                start = end;
                SkipWs(ref start);
                if (start == _source.Length || _source[start] != '>')
                    continue;

                return element == value
                    ? retval
                    : -1;
            }
        }

        void ConsumeTo(int end)
        {
            Consume(end - _index);
        }

        void Consume(int count = 1)
        {
            switch (count)
            {
                case 0:
                    break;
                case 1:
                    _sb.Append(_source[_index++]);
                    break;
                default:
                    _sb.Append(_source, _index, count);
                    _index += count;
                    break;
            }
        }

        void SkipWs(ref int i)
        {
            while (i < _source.Length && char.IsWhiteSpace(_source[i]))
                i++;
        }

        void ScanWs()
        {
            int end = _index;
            SkipWs(ref end);
            ConsumeTo(end);
        }

        void Scan(string terminal)
        {
            var i = _source.IndexOf(terminal, _index, StringComparison.Ordinal);
            ConsumeTo(i == -1 ? _source.Length : i + terminal.Length);
        }

        bool Scan(char terminal)
        {
            var i = _source.UnquotedIndexOf(terminal, _index);

            if (i == -1)
            {
                ConsumeTo(_source.Length);
                return false;
            }

            // Fail on malformed markup, don't consume
            if (terminal == '>')
                for (int j = _index; j < i; j++)
                    if (_source[j] == '<')
                        return false;

            ConsumeTo(i + 1);
            return true;
        }

        string ParseName()
        {
            ScanWs();
            if (_index == _source.Length)
                return null;

            var start = _index;
            while (_index < _source.Length && IsNameChar(_source[_index]))
                _index++;

            if (start == _index)
                return "";

            var retval = _source.Substring(start, _index - start);
            _sb.Append(retval);
            return retval;
        }

        bool IsNameChar(char c)
        {
            switch (c)
            {
                case '.':
                case ':':
                case '-':
                case '_':
                case '!':
                case '[':
                case ']':
                case '(':
                case ')':
                case '{':
                case '}':
                case '*':
                    return true;
                default:
                    return char.IsLetterOrDigit(c);
            }
        }
    }
}