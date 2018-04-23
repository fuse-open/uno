using System.Collections.Generic;

namespace Uno.Compiler.Frontend.Analysis
{
    public class Lexer
    {
        static readonly int[][] Transitions = new LexerBuilder().Build();
        static readonly int[] DecimalPeriodState = Transitions[LexerBuilder.ScanDecimalPeriod];
        static readonly int[] RootState = Transitions[0];

        public static List<Token> Tokenize(string filename, string text)
        {
            return Tokenize(new SourceFile(filename, text), text);
        }

        public static List<Token> Tokenize(SourceFile file, string text)
        {
            var lexer = new Lexer(file, text);
            lexer.Tokenize();
            lexer.Done();
            return lexer._tokens;
        }

        const char Nul = (char) 0;
        const char Mask = (char) 0xFF80;

        readonly SourceFile _file;
        readonly List<Token> _tokens;
        readonly string _text;
        readonly int _length;
        int _tokenIndex;
        int _tokenLineNumber;
        int _tokenLinePosition;
        int _lineNumber;
        int _linePosition;
        int _index;

        Lexer(SourceFile file, string text)
        {
            _file = file;
            _text = text;
            _length = text.Length;
            _lineNumber = file.StartLine;
            _linePosition = file.StartColumn - 1;

            // Reserve some memory to avoid resizing in preferred cases (nTOKENS < [nCHARS / 4 + 32])
            _tokens = new List<Token>(_text.Length >> 2 + 32);
        }

        void Tokenize()
        {
            for (var c = Read();;)
            {
            NEXT_TOKEN:
                _tokenIndex = _index;
                _tokenLineNumber = _lineNumber;
                _tokenLinePosition = _linePosition;

                for (var state = RootState;;)
                {
                    var next = state[(c & Mask) == 0 ? c : LexerBuilder.Utf16Char];

                SWITCH_TOKEN:
                    var token = (TokenType) (next >> 16);
                    switch (token)
                    {
                        case 0:
                        {
                            state = Transitions[next];
                            c = Read();
                            continue;
                        }
                        case TokenType.Invalid:
                        {
                            // End of file
                            if (_tokenIndex >= _length)
                                return;

                            // Advanced whitespace
                            if (IsAdvancedWhitespace(c) || c == Nul)
                            {
                                // Eat
                                c = Read();
                                // Skip
                                goto NEXT_TOKEN;
                            }

                            // Unicode identifier OR escape sequence
                            var first = _text[_tokenIndex - 1];
                            if ((char.IsLetter(c) || c == '\\') && (
                                    _tokenIndex == _index ||
                                    LexerBuilder.IdentifierChars[first] && 
                                        !LexerBuilder.DecimalChars[first]))
                            {
                                // Keep scanning
                                for (c = Read();
                                    (c & Mask) == 0 && LexerBuilder.IdentifierChars[c] ||
                                        char.IsLetter(c) || 
                                        c == '\\';)
                                    c = Read();

                                Emit(TokenType.Identifier);
                                goto NEXT_TOKEN;
                            }

                            goto INVALID_TOKEN;
                        }
                        case TokenType.Whitespace:
                        {
                            // Skip whitespace
                            goto NEXT_TOKEN;
                        }
                        case TokenType.SingleLineComment:
                        {
                            for (var doc = c == '/';; c = Read())
                            {
                            NEXT_COMMENT:
                                switch (c)
                                {
                                    case '\n':
                                        // Skip whitespace
                                        for (c = Read();; c = Read())
                                            if (!((c & Mask) == 0 && LexerBuilder.WhitespaceChars[c]) &&
                                                    !IsAdvancedWhitespace(c))
                                                break;

                                        // Skip following single-line comments
                                        if (c == '/' && Peek() == '/')
                                        {
                                            Read();
                                            c = Read();
                                            goto NEXT_COMMENT;
                                        }

                                        if (doc)
                                            Emit(TokenType.DocComment);
                                        goto NEXT_TOKEN;
                                    case Nul:
                                        if (_index < _length)
                                            continue;
                                        return;
                                }
                            }
                        }
                        case TokenType.MultiLineComment:
                        {
                            for (var doc = c == '*' && Peek() != '/';;)
                            {
                                switch (c)
                                {
                                    case '*':
                                        c = Read();
                                        if (c != '/')
                                            continue;
                                        c = Read();
                                        if (doc)
                                            Emit(TokenType.DocComment);
                                        goto NEXT_TOKEN;
                                    case Nul:
                                        if (_index < _length)
                                            break;
                                        goto UNEXPECTED_EOF;
                                }

                                c = Read();
                            }
                        }
                        case TokenType.PreprocessorDirective:
                        {
                            // Find '#' index
                            var hash = _index - 2;
                            while (hash > 0 && _text[hash] != '#')
                                hash--;

                            // Check that only whitespace precedes directive
                            for (var i = hash - 1; i >= 0; i--)
                            {
                                var d = _text[i];
                                if (d == '\n')
                                    break;
                                if ((d & Mask) == 0 && LexerBuilder.WhitespaceChars[d])
                                    continue;
                                goto INVALID_TOKEN;
                            }

                            // TODO: We need to care about multiline comments here (and strings)
                            for (;; c = Read())
                            {
                                switch (c)
                                {
                                    case '\n':
                                        Emit(TokenType.PreprocessorDirective);
                                        c = Read();
                                        goto NEXT_TOKEN;
                                    case Nul:
                                        if (_index < _length)
                                            continue;
                                        Emit(TokenType.PreprocessorDirective);
                                        return;
                                }
                            }
                        }
                        case TokenType.AlphaBlock:
                        {
                            for (var q = Nul;; c = Read())
                            {
                            ALPHA_BLOCK:
                                switch (c)
                                {
                                    case '@':
                                        c = Read();
                                        if (c != '}' || q != Nul)
                                            continue;
                                        c = Read();
                                        Emit(TokenType.AlphaBlock);
                                        goto NEXT_TOKEN;
                                    case '"':
                                    case '\'':
                                        if (q == c)
                                            q = Nul;
                                        else if (q == Nul)
                                            q = c;
                                        continue;
                                    case '\\':
                                        Read();
                                        continue;
                                    case '\n':
                                        q = Nul;
                                        continue;
                                    case '/':
                                        if (q != 0)
                                            continue;
                                        switch (Peek())
                                        {
                                            case '*':
                                                Read();
                                                for (c = Read();;)
                                                {
                                                    switch (c)
                                                    {
                                                        case '*':
                                                            c = Read();
                                                            if (c != '/')
                                                                continue;
                                                            c = Read();
                                                            goto ALPHA_BLOCK;
                                                        case Nul:
                                                            if (_index < _length)
                                                                break;
                                                            goto UNEXPECTED_EOF;
                                                    }
                                                    c = Read();
                                                }
                                            case '/':
                                                Read();
                                                for (c = Read();;)
                                                {
                                                    switch (c)
                                                    {
                                                        case '\n':
                                                            c = Read();
                                                            goto ALPHA_BLOCK;
                                                        case Nul:
                                                            if (_index < _length)
                                                                break;
                                                            goto UNEXPECTED_EOF;
                                                    }
                                                    c = Read();
                                                }
                                            }
                                        continue;
                                    case Nul:
                                        if (_index < _length)
                                            continue;
                                        goto UNEXPECTED_EOF;
                                }
                            }
                        }
                        case TokenType.AlphaExpression:
                        {
                            var pc = 0;
                            for (var q = Nul;; c = Read())
                            {
                                switch (c)
                                {
                                    case '(':
                                        if (q == Nul)
                                            pc++;
                                        continue;
                                    case ')':
                                        if (pc == 0 && q == Nul)
                                        {
                                            c = Read();
                                            Emit(TokenType.AlphaExpression);
                                            goto NEXT_TOKEN;
                                        }
                                        if (q == Nul)
                                            pc--;
                                        continue;
                                    case '"':
                                    case '\'':
                                        if (q == c)
                                            q = Nul;
                                        else if (q == Nul)
                                            q = c;
                                        continue;
                                    case '\\':
                                        Read();
                                        continue;
                                    case '\n':
                                        q = Nul;
                                        continue;
                                    case Nul:
                                        if (_index < _length)
                                            continue;
                                        goto UNEXPECTED_EOF;
                                }
                            }
                        }
                        case TokenType.AlphaString:
                        {
                            for (;; c = Read())
                            {
                                switch (c)
                                {
                                    case '\\':
                                        Read();
                                        continue;
                                    case '"':
                                        c = Read();
                                        Emit(TokenType.AlphaString);
                                        goto NEXT_TOKEN;
                                    case '\n':
                                        goto INVALID_TOKEN;
                                    case Nul:
                                        if (_index < _length)
                                            continue;
                                        goto UNEXPECTED_EOF;
                                }
                            }
                        }
                        case TokenType.DollarString:
                        {
                            for (;; c = Read())
                            {
                                switch (c)
                                {
                                    case '\\':
                                        Read();
                                        continue;
                                    case '"':
                                        c = Read();
                                        Emit(TokenType.DollarString);
                                        goto NEXT_TOKEN;
                                    case '\n':
                                        goto INVALID_TOKEN;
                                    case Nul:
                                        if (_index < _length)
                                            continue;
                                        goto UNEXPECTED_EOF;
                                }
                            }
                        }
                        case TokenType.StringLiteral:
                        {
                            for (;; c = Read())
                            {
                                switch (c)
                                {
                                    case '\\':
                                        Read();
                                        continue;
                                    case '"':
                                        c = Read();
                                        Emit(TokenType.StringLiteral);
                                        goto NEXT_TOKEN;
                                    case '\n':
                                        goto INVALID_TOKEN;
                                    case Nul:
                                        if (_index < _length)
                                            continue;
                                        goto UNEXPECTED_EOF;
                                }
                            }
                        }
                        case TokenType.CharLiteral:
                        {
                            for (;; c = Read())
                            {
                                switch (c)
                                {
                                    case '\\':
                                        Read();
                                        continue;
                                    case '\'':
                                        c = Read();
                                        Emit(TokenType.CharLiteral);
                                        goto NEXT_TOKEN;
                                    case '\n':
                                        goto INVALID_TOKEN;
                                    case Nul:
                                        if (_index < _length)
                                            continue;
                                        goto UNEXPECTED_EOF;
                                }
                            }
                        }
                        case TokenType.DecimalLiteral:
                        {
                            if (c == '.')
                            {
                                var d = Peek();
                                next = DecimalPeriodState[(d & Mask) == 0 ? d : 0];

                                // Switch to DOUBLE or FLOAT on INT \.[0-9](dDeEfF)?
                                if (next != LexerBuilder.Invalid)
                                {
                                    c = Read();
                                    goto SWITCH_TOKEN;
                                }
                            }

                            Emit(token);
                            goto NEXT_TOKEN;
                        }
                        default:
                        {
                            Emit(token);
                            goto NEXT_TOKEN;
                        }
                    }
                }

            INVALID_TOKEN:
                c = Read();
                Emit(TokenType.Invalid);
            }

        UNEXPECTED_EOF:
            Emit(TokenType.Invalid);
        }

        void Done()
        {
            _tokens.Add(new Token(_file, _text, TokenType.EndOfFile, 
                _lineNumber, _linePosition + 1, _length, _length));
        }

        void Emit(TokenType type)
        {
            _tokens.Add(
                new Token(
                    _file,
                    _text,
                    type,
                    _tokenLineNumber,
                    _tokenLinePosition,
                    // Subtract one becuase we're always one Read() ahead
                    _tokenIndex - 1,
                    _index - 1));
        }

        char Peek()
        {
            return _index < _length
                ? _text[_index]
                : Nul;
        }

        char Read()
        {
            if (_index >= _length)
            {
                // Keep going to avoid off-by-one on last token
                _index++;
                return Nul;
            }

            var retval = _text[_index++];
            switch (retval)
            {
                case '\n':
                    _lineNumber++;
                    _linePosition = 0;
                    return retval;
                default:
                    _linePosition++;
                    return retval;
            }
        }

        bool IsAdvancedWhitespace(char c)
        {
            switch (c)
            {
                case '\u0009': // CHARACTER TABULATION
                case '\u000A': // LINE FEED
                case '\u000B': // LINE TABULATION
                case '\u000C': // FORM FEED
                case '\u000D': // CARRIAGE RETURN
                case '\u0020': // SPACE
                case '\u00A0': // NO-BREAK SPACE
                case '\u2000': // EN QUAD
                case '\u2001': // EM QUAD
                case '\u2002': // EN SPACE
                case '\u2003': // EM SPACE
                case '\u2004': // THREE-PER-EM SPACE
                case '\u2005': // FOUR-PER-EM SPACE
                case '\u2006': // SIX-PER-EM SPACE
                case '\u2007': // FIGURE SPACE
                case '\u2008': // PUNCTUATION SPACE
                case '\u2009': // THIN SPACE
                case '\u200A': // HAIR SPACE
                case '\u200B': // ZERO WIDTH SPACE
                case '\u3000': // IDEOGRAPHIC SPACE
                case '\uFEFF': // ZERO WIDTH NO-BREAK SPACE
                    return true;
                default:
                    return false;
            }
        }
    }
}
