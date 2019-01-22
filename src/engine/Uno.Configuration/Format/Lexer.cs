using System.Collections.Generic;
using System.Text;

namespace Uno.Configuration.Format
{
    public class Lexer
    {
        public static void Tokenize(string source, IList<Token> result, out bool unexpectedEof)
        {
            var lexer = new Lexer(source);

            Token token;
            while (lexer.ReadToken(out token))
                result.Add(token);

            unexpectedEof = lexer._unexpectedEof;
        }

        const char Eof = (char)0xffff;
        readonly string _source;
        bool _isConditional;
        bool _unexpectedEof;
        int _lineNumber = 1;
        int _linePosition;
        int _index;

        Lexer(string source)
        {
            _source = source;
        }

        bool ReadToken(out Token result)
        {
            char c = Read();
            FindToken(ref c);
            result = new Token(_lineNumber, _linePosition);

            if (c == Eof)
                return false;

            if (!_isConditional && c == '"')
            {
                var sb = new StringBuilder();

                // String literal
                for (;;)
                {
                    switch (c = Read())
                    {
                        case Eof:
                            _unexpectedEof = true;
                            result.Type = TokenType.String;
                            result.String = sb.ToString();
                            return true;
                        case '"':
                            result.Type = TokenType.String;
                            result.String = sb.ToString();
                            return true;
                        case '\\':
                            sb.Append(Read().Unescape());
                            break;
                        default:
                            sb.Append(c);
                            break;
                    }
                }
            }

            if (!_isConditional && c == '`')
            {
                var sb = new StringBuilder();

                // String literal
                for (;;)
                {
                    switch (c = Read())
                    {
                        case Eof:
                            _unexpectedEof = true;
                            result.Type = TokenType.String;
                            result.String = sb.ToString();
                            return true;
                        case '`':
                            result.Type = TokenType.String;
                            result.String = sb.ToString();
                            return true;
                        default:
                            sb.Append(c);
                            break;
                    }
                }
            }

            var cWasEscaped = false;
            if (!_isConditional && c == '\\' && Peek() == ' ')
            {
                c = Read();
                cWasEscaped = true;
            }

            if (cWasEscaped || c.IsKey(_isConditional))
            {
                var sb = new StringBuilder();
                sb.Append(c);

                // Unquoted string: LETTER DIGIT . _ - * ~ / @ $ % ( )
                for (;;)
                {
                    var d = Peek();
                    if (d.IsKey(_isConditional))
                        sb.Append(Read());
                    else if (!_isConditional && d == '\\' && Peek(1) == ' ')
                    {
                        // Escaped space
                        sb.Append(' ');
                        Read();
                        Read();
                    }
                    else
                        break;
                }

                result.String = sb.ToString();

                // Keyword
                switch (result.String)
                {
                    case "include":
                    case "require":
                        result.Type = TokenType.Require;
                        return true;
                    case "if":
                        _isConditional = true;
                        result.Type = TokenType.If;
                        return true;
                    case "else":
                        result.Type = TokenType.Else;
                        return true;
                    case "null":
                        result.Type = TokenType.Null;
                        return true;
                    default:
                        result.Type = TokenType.String;
                        return true;
                }
            }

            switch (c)
            {
                case '+':
                    return ReadOperator(TokenType.Append, '=', ref result);
                case '&':
                    return ReadOperator(TokenType.And, c, ref result);
                case '|':
                    return ReadOperator(TokenType.Or, c, ref result);
                case '{':
                case ',':
                    _isConditional = false;
                    break;
            }

            result.Type = TokenType.Punctuation;
            result.Punctuation = c;
            return true;
        }

        bool ReadOperator(TokenType op, char c, ref Token result)
        {
            if (Peek() == c)
            {
                result.Type = op;
                result.String = new string(new[] {Peek(-1), Read()});
            }
            else
            {
                result.Type = TokenType.Punctuation;
                result.Punctuation = c;
            }

            return true;
        }

        void FindToken(ref char c)
        {
            for (;;)
            {
                switch (c)
                {
                    case ' ':
                    case '\r':
                    case '\t':
                        // Skip whitespace
                        c = Read();
                        continue;
                    case '\n':
                        _isConditional = false;
                        c = Read();
                        continue;
                    case '/':
                        switch (Peek())
                        {
                            case '/':
                                // Single line comment
                                for (Read();;)
                                {
                                    switch (Read())
                                    {
                                        case '\n':
                                            c = Read();
                                            FindToken(ref c);
                                            return;
                                        case Eof:
                                            return;
                                    }
                                }

                            case '*':
                                // Multi-line comment
                                for (Read();;)
                                {
                                    switch (Read())
                                    {
                                        case '*':
                                            c = Read();
                                            if (c != '/')
                                                continue;
                                            c = Read();
                                            FindToken(ref c);
                                            return;
                                        case Eof:
                                            _unexpectedEof = true;
                                            return;
                                    }
                                }

                            default:
                                return;
                        }
                    default:
                        return;
                }
            }
        }

        char Peek(int offset = 0)
        {
            var index = _index + offset;
            return index < _source.Length
                ? _source[index]
                : Eof;
        }

        char Read()
        {
            if (_index >= _source.Length)
                return Eof;

            var retval = _source[_index++];

            if (retval == '\n')
            {
                _lineNumber++;
                _linePosition = 1;
            }
            else
                _linePosition++;

            return retval;
        }
    }
}
