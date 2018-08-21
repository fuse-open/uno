using Uno;
using Uno.Collections;
using Uno.IO;
using Uno.Text;

namespace Uno.Data.Json
{
    class Parser
    {
        TextReader _json;

        Parser(TextReader json)
        {
            _json = json;
        }

        public static Value Parse(TextReader json)
        {
            var p = new Parser(json);
            var v = p.ParseValue();
            p.SkipWhiteSpace();
            if (!p.Eof()) throw new JsonException("Expected end of file");
            return v;
        }

        Value ParseValue()
        {
            if (Eof()) return Null.Singleton;

            SkipWhiteSpace();

            char c = Peek();

            switch (c)
            {
                case '"': return ParseString();
                case '{': return ParseObject();
                case '[': return ParseArray();
                case 'f': return ParseFalse();
                case 't': return ParseTrue();
                case 'n': return ParseNull();
                default:
                    if (IsNumericStartChar(c))
                        return ParseNumber();
                    throw UnexpectedCharacter(c);
            }
        }

        void ParseKeyword(string keyword)
        {
            for (int i = 0; i < keyword.Length; ++i)
            {
                var ch = Consume();
                if (keyword[i] != ch)
                    throw UnexpectedCharacter(ch);
            }
        }

        Boolean ParseFalse()
        {
            ParseKeyword("false");
            return Boolean.False;
        }

        Boolean ParseTrue()
        {
            ParseKeyword("true");
            return Boolean.True;
        }

        Null ParseNull()
        {
            ParseKeyword("null");
            return Null.Singleton;
        }

        Number ParseNumber()
        {
            var sb = new StringBuilder();

            if (Peek() == '-')
                sb.Append(Consume());

            var c = Peek();
            if (!char.IsDigit(c))
                throw UnexpectedCharacter(c);

            sb.Append(c);
            Advance();

            if (c != '0')
            {
                while (!Eof() && char.IsDigit(Peek()))
                    sb.Append(Consume());
            }

            if (!Eof() && Peek() == '.')
            {
                sb.Append(Consume());

                if (!char.IsDigit(Peek()))
                    throw UnexpectedCharacter(Peek());

                while (!Eof() && char.IsDigit(Peek()))
                    sb.Append(Consume());
            }

            if (!Eof() && char.ToLower(Peek()) == 'e')
            {
                sb.Append(Consume());

                if (Peek() == '+' || Peek() == '-')
                    sb.Append(Consume());

                if (!char.IsDigit(Peek()))
                    throw UnexpectedCharacter(Peek());

                while (!Eof() && char.IsDigit(Peek()))
                    sb.Append(Consume());
            }

            return new Number(double.Parse(sb.ToString()));
        }

        bool IsNumericStartChar(char c)
        {
            return char.IsDigit(c) || c == '-';
        }

        Array ParseArray()
        {
            Advance();
            var a = new Array();

            while (!Eof())
            {
                SkipWhiteSpace();

                var c = Peek();
                if (c == ']') { Advance(); return a; }

                var v = ParseValue();
                a.Add(v);

                SkipWhiteSpace();
                c = Peek();
                if (c == ',') { Advance(); continue; }
                else if (c == ']') { Advance(); return a; }
                else throw UnexpectedCharacter(c);
            }

            throw UnexpectedEndOfFile();
        }

        Object ParseObject()
        {
            Advance();
            var obj = new Object();

            while (!Eof())
            {
                SkipWhiteSpace();
                var c = Peek();
                if (c == '}') { Advance(); return obj; }

                var key = ParseRawString();

                SkipWhiteSpace();
                c = Peek();

                if (c != ':') throw new Exception("Expected ':'");
                Advance();

                var val = ParseValue();
                obj.Add(key, val);

                SkipWhiteSpace();
                c = Peek();
                if (c == ',') { Advance(); continue; }
                else if (c == '}') { Advance(); return obj; }
                else throw UnexpectedCharacter(c);
            }

            throw UnexpectedEndOfFile();
        }

        String ParseString()
        {
            return new String(ParseRawString());
        }

        string ParseRawString()
        {
            var ch = Consume();
            if (ch != '\"')
                throw UnexpectedCharacter(ch);

            var sb = new StringBuilder();
            ch = Consume();
            while (ch != '\"')
            {
                if (ch == '\\')
                {
                    // escape character

                    var ch2 = Consume();
                    switch (ch2)
                    {
                        case '\\': sb.Append('\\'); break;
                        case '/': sb.Append('/'); break;
                        case '"': sb.Append('\"'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'u':
                            {
                                uint value = 0;
                                for (int i = 0; i < 4; ++i)
                                {
                                    var ch3 = Consume();
                                    if (!IsHexValue(ch3))
                                        throw UnexpectedCharacter(ch3);

                                    value = (value << 4) | GetHexValue(ch3);
                                }

                                sb.Append((char)value);
                            }
                            break;

                        default:
                            sb.Append(ch);
                            sb.Append(ch2);
                            break;
                    }
                }
                else
                {
                    sb.Append(ch);
                }

                ch = Consume();
            }

            return sb.ToString();
        }

        static bool IsHexValue(char ch)
        {
            return (ch >= '0' && ch <= '9') ||
                   (ch >= 'a' && ch <= 'f') ||
                   (ch >= 'A' && ch <= 'F');
        }

        static uint GetHexValue(char hexChar)
        {
            if ((uint)hexChar >= 65)
                return (uint)hexChar - 55;
            else
                return (uint)hexChar - 48;
        }

        bool Eof()
        {
            return _json.Peek() < 0;
        }

        void SkipWhiteSpace()
        {
            while (!Eof())
            {
                var c = Peek();
                switch (c)
                {
                    case ' ':
                    case '\t':
                    case '\n':
                    case '\r':
                        Advance();
                        break;

                    default:
                        return;
                }
            }
        }

        char Peek()
        {
            var ret = _json.Peek();
            if (ret < 0)
                throw UnexpectedEndOfFile();

            return (char)ret;
        }

        char Consume()
        {
            var ret = _json.Read();
            if (ret < 0)
                throw UnexpectedEndOfFile();

            return (char)ret;
        }

        void Advance()
        {
            _json.Read();
        }

        static JsonException UnexpectedEndOfFile()
        {
            return new JsonException("Unexpected end of file");
        }

        static JsonException UnexpectedCharacter(char c)
        {
            return new JsonException("Unexpected character: " + c);
        }
    }
}
