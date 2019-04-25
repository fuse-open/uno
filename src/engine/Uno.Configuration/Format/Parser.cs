using System;
using System.Collections.Generic;
using Uno.IO;

namespace Uno.Configuration.Format
{
    class Parser : List<Token>
    {
        readonly StuffFile _file;
        readonly bool _unexpectedEof;
        int _index;

        internal Parser(StuffFile file, string stuff)
        {
            _file = file;
            Lexer.Tokenize(stuff, this, out _unexpectedEof);
        }

        bool IsEndOfFile => _index >= Count;

        internal void Parse()
        {
            var isObject = TryParse('{');
            ParseRoot(true, "");

            if (isObject)
                Parse('}', "Expected '}' to end object");
            if (!IsEndOfFile)
                throw SyntaxError(Peek(), "Expected string or 'if'");
            if (_unexpectedEof)
                throw SyntaxError(Peek(), "Unexpected EOF");
        }

        void ParseRoot(bool cond, string prefix)
        {
            while (_index < Count)
                if (!TryParseKeyValues(cond, prefix) &&
                    !TryParseIfElse(cond, prefix) &&
                    !TryParseRequire(cond, prefix))
                    break;
        }

        bool TryParseKeyValues(bool cond, string prefix)
        {
            for (;;)
            {
                Token token;
                if (!TryParse(TokenType.String, out token))
                    return false;

                var key = prefix + token.String;
                var type = StuffItemType.None;

                if (TryParse(TokenType.Append, out token))
                    type = StuffItemType.Append;
                else if (Peek().Punctuation != '{')
                    Parse(':', "Expected ':', '+=' or '{' after key");

                if (type == StuffItemType.None && TryParse('{'))
                {
                    ParseRoot(cond, key + ".");
                    Parse('}', "Expected '}' to end object");
                }
                else
                {
                    var line = Peek().LineNumber;
                    var value = ParseValue();

                    if ((!TryParseSameLineIf() || ParseExpression()) &&
                        (cond || _file.Defines == null))
                        _file.Add(key, value, line, type);
                }

                // Newline, comma or right curly brace
                if (!TryParse(',') && Peek().Punctuation != '}')
                    ParseEndOfLine("Expected newline or ',' to end key/value");
            }
        }

        bool TryParseIfElse(bool parentCond, string prefix)
        {
            Token token;
            if (!TryParse(TokenType.If, out token))
                return false;

            var cond = ParseExpression();
            Parse('{', "Expected '{' after if EXPRESSION");
            ParseRoot(parentCond && cond || _file.Defines == null, prefix);
            Parse('}', "Expected '}' to end if block");

            if (TryParse(TokenType.Else, out token))
            {
                if (TryParseIfElse(parentCond && !cond, prefix))
                    return true;

                Parse('{', "Expected '{' after else");
                ParseRoot(parentCond && !cond || _file.Defines == null, prefix);
                Parse('}', "Expected '}' to end else block");
            }

            return true;
        }

        bool TryParseRequire(bool parentCond, string prefix)
        {
            Token token;
            if (!TryParse(TokenType.Require, out token))
                return false;

            if (!string.IsNullOrEmpty(prefix))
                throw SyntaxError(token, "'require' is not supported inside an object");

            if (!TryParse(TokenType.String, out token))
                throw SyntaxError(token, "Expected a filename to 'require'");

            if (parentCond || _file.Defines == null)
                _file.Add(null, token.String, token.LineNumber, StuffItemType.Require);

            ParseEndOfLine("Expected newline to end 'require'");
            return true;
        }

        bool ParseExpression(Precedence prec = 0)
        {
            var result = ParsePrimary();

            for (;;)
            {
                switch (Peek().Type)
                {
                    case TokenType.And:
                        if (prec > Precedence.And)
                            return result;
                        Read();
                        result = ParseExpression(Precedence.And) && result;
                        continue;
                    case TokenType.Or:
                        if (prec > Precedence.Or)
                            return result;
                        Read();
                        result = ParseExpression(Precedence.Or) || result;
                        continue;
                }

                return result;
            }
        }

        bool ParsePrimary()
        {
            var token = Read();

            switch (token.Type)
            {
                case TokenType.String:
                {
                    if (!token.String.IsIdentifier())
                        throw SyntaxError(token, "Expected identifier");

                    return _file.Defines == null || _file.Defines.Contains(token.String.ToUpperInvariant());
                }
                case TokenType.Punctuation:
                {
                    switch (token.Punctuation)
                    {
                        case '!':
                            return !ParseExpression(Precedence.Not) || _file.Defines == null;
                        case '(':
                        {
                            var result = ParseExpression();
                            Parse(')', "Expected ')' to end sub expression");
                            return result;
                        }
                    }
                    break;
                }
            }

            throw SyntaxError(token, "Expected conditional expression");
        }

        string ParseValue()
        {
            string result;
            if (TryParseValue(out result))
                return result;

            throw SyntaxError(Peek(), "Expected string or '[' to specify value");
        }

        bool TryParseValue(out string result)
        {
            Token token;
            if (TryParse(TokenType.String, out token))
            {
                result = token.String;
                return true;
            }
            if (TryParse(TokenType.Null, out token))
            {
                result = null;
                return true;
            }
            if (TryParse('['))
            {
                var items = new List<string>();
                while (TryParseValue(out result))
                {
                    if (!TryParseSameLineIf() || ParseExpression())
                        items.Add(result);
                    // Optional comma in arrays
                    TryParse(',');
                }

                Parse(']', "Expected ']' to end array");
                result = string.Join("\n", items);
                return true;
            }

            result = null;
            return false;
        }

        void Parse(char p, string error)
        {
            if (!TryParse(p))
                throw SyntaxError(Peek(), error);
        }

        bool TryParse(char p)
        {
            var token = Peek();
            if (token.Punctuation != p)
                return false;

            _index++;
            return true;
        }

        bool TryParse(TokenType type, out Token token)
        {
            token = Peek();
            if (token.Type != type)
                return false;

            _index++;
            return true;
        }

        bool TryParseSameLineIf()
        {
            Token token;
            return !IsEndOfFile &&
                   this[_index].LineNumber == this[_index - 1].LineNumber &&
                   TryParse(TokenType.If, out token);
        }

        void ParseEndOfLine(string error)
        {
            if (!IsEndOfFile && this[_index].LineNumber <= this[_index - 1].LineNumber)
                throw SyntaxError(this[_index], error);
        }

        Token Peek()
        {
            return _index < Count
                ? this[_index]
                : Read();
        }

        Token Read()
        {
            if (_index < Count)
                return this[_index++];

            return new Token(_index > 0 ? this[_index - 1].LineNumber + 1 : 1, 1)
            {
                Type = 0,
                String = "<EOF>",
                Punctuation = '\0'
            };
        }

        FormatException SyntaxError(Token token, string message)
        {
            return new FormatException(_file.Filename.ToRelativePath() + "(" + token.LineNumber + "." + token.LinePosition + "): " + message + " (" + token + ")");
        }

        enum Precedence
        {
            Or = 1,
            And,
            Not
        }
    }
}