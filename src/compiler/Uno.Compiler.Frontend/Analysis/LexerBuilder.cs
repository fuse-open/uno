using System;
using System.Collections.Generic;
using Uno.Collections;

namespace Uno.Compiler.Frontend.Analysis
{
    class LexerBuilder
    {
        public const char Nul = (char) 0;
        public const char Max = (char) 0x80;
        public const char Utf16Char = (char) 0x7f;
        public const int ScanDecimalPeriod = 1;
        public const int ScanIdentifier = 2;
        public const int ScanWhitespace = 3;
        public const int EmitDouble = (int) TokenType.DoubleLiteral << 16;
        public const int EmitIdentifier = (int) TokenType.Identifier << 16;
        public const int EmitPreprocessorDirective = (int) TokenType.PreprocessorDirective << 16;
        public const int Invalid = (int) TokenType.Invalid << 16;

        public static readonly bool[] IdentifierChars = GetIdentifierChars();
        public static readonly bool[] DecimalChars = GetDecimalChars();
        public static readonly bool[] HexadecimalChars = GetHexadecimalChars();
        public static readonly bool[] OctalChars = GetOctalChars();
        public static readonly bool[] BinaryChars = GetBinaryChars();
        public static readonly bool[] WhitespaceChars = GetWhitespaceChars();

        readonly List<int[]> _list = new List<int[]> {null, null};

        public int[][] Build()
        {
            var identifier = Create(TokenType.Identifier);
            var whitespace = Create(TokenType.Whitespace);
            _list.Add(identifier);
            _list.Add(whitespace);

            var scanNumber = Build(TokenType.DecimalLiteral, DecimalChars);
            var scanDouble = Build(TokenType.DoubleLiteral, DecimalChars);
            BuildDouble(scanNumber, scanDouble);
            BuildInt(TokenType.DecimalLiteral, scanNumber);
            BuildInt(TokenType.HexadecimalLiteral, scanNumber, "xX", HexadecimalChars);
            BuildInt(TokenType.OctalLiteral, scanNumber, "oO", OctalChars);
            BuildInt(TokenType.BinaryLiteral, scanNumber, "bB", BinaryChars);

            var map = new ListDictionary<char, Tuple<string, TokenType>>();
            foreach (var e in Tokens.Reserved)
                map.Add(e.Key[0], Tuple.Create(e.Key, e.Value));

            var root = _list[0] = Build(map);
            var alpha = _list[root['@']];
            var dot = _list[root['.']];

            for (int j = Nul; j < Max; j++)
            {
                if (WhitespaceChars[j])
                    root[j] = ScanWhitespace;
                else if (root[j] == Invalid &&
                         IdentifierChars[j] && !DecimalChars[j])
                    root[j] = ScanIdentifier;
                else if (DecimalChars[j])
                    root[j] = scanNumber;

                if (IdentifierChars[j])
                    identifier[j] = ScanIdentifier;
                else if (WhitespaceChars[j])
                    whitespace[j] = ScanWhitespace;

                if (DecimalChars[j])
                    dot[j] = scanDouble;
                else if (IdentifierChars[j])
                    alpha[j] = ScanIdentifier;
            }

            // Goto invalid state on keyword following UTF-16 char,
            // the Lexer class will special case this.
            identifier[Utf16Char] = Invalid;

            return _list.ToArray();
        }

        int[] Build(ListDictionary<char, Tuple<string, TokenType>> map, char c = Nul, int i = 0)
        {
            var state = Create();

            foreach (var e in map)
                state[e.Key] = Build(e.Key, e.Value, i + 1);

            if (IdentifierChars[c])
                for (int j = Nul; j < Max; j++)
                    if (state[j] == Invalid)
                        state[j] = IdentifierChars[j]
                            ? ScanIdentifier
                            : EmitIdentifier;

            // Goto invalid state on keyword following UTF-16 char
            // the Lexer class will special case this.
            state[Utf16Char] = Invalid;

            return state;
        }

        int Build(char c, List<Tuple<string, TokenType>> tokens, int i)
        {
            var map = new ListDictionary<char, Tuple<string, TokenType>>();

            foreach (var e in tokens)
                if (e.Item1.Length > i)
                    map.Add(e.Item1[i], e);

            var state = Build(map, c, i);
            var retval = _list.Count;
            _list.Add(state);

            foreach (var e in tokens)
                if (e.Item1.Length == i)
                    for (int j = Nul; j < Max; j++)
                        if ((state[j] == Invalid || state[j] == EmitIdentifier) &&
                            (!char.IsLetter(c) || !IdentifierChars[j]))
                            state[j] = (int) e.Item2 << 16;

            return retval;
        }

        void BuildInt(TokenType type, int parent, IEnumerable<char> next, bool[] chars)
        {
            BuildInt(type, Build(type, parent, next, chars));
        }

        void BuildInt(TokenType type, int parent)
        {
            Build(type, Build(type, parent, "lL"), "uU");
            Build(type, Build(type, parent, "uU"), "lL");
        }

        void BuildDouble(int scanNumber, int scanDouble)
        {
            var root = _list[ScanDecimalPeriod] = Create(TokenType.DoubleLiteral);
            DecimalChars['+'] = DecimalChars['-'] = true;

            foreach (var p in new[]
            {
                ScanDecimalPeriod,
                scanNumber,
                scanDouble,
                Build(TokenType.DoubleLiteral, ScanDecimalPeriod, "eE", DecimalChars),
                Build(TokenType.DoubleLiteral, scanNumber, "eE", DecimalChars),
                Build(TokenType.DoubleLiteral, scanDouble, "eE", DecimalChars)
            })
            {
                Build(TokenType.FloatLiteral, p, "fF");
                Build(TokenType.DoubleLiteral, p, "dD");
                var a = Build(TokenType.Invalid, p, "pP");
            }

            DecimalChars['+'] = DecimalChars['-'] = false;

            for (int j = Nul; j < Max; j++)
                if (DecimalChars[j])
                    root[j] = ScanDecimalPeriod;
                else if (IdentifierChars[j] && 
                        root[j] == EmitDouble)
                    root[j] = Invalid;
        }

        int Build(TokenType type, int parent, IEnumerable<char> next, bool[] chars = null, int invalid = Invalid)
        {
            var retval = Build(type, chars, invalid);
            var state = _list[parent];

            foreach (var c in next)
                state[c] = retval;

            return retval;
        }

        int Build(TokenType type, bool[] chars = null, int invalid = Invalid)
        {
            var state = Create(type);
            var retval = _list.Count;
            _list.Add(state);

            for (int j = Nul; j < Max; j++)
                if (chars != null && chars[j])
                    state[j] = retval;
                else if (IdentifierChars[j])
                    state[j] = invalid;

            return retval;
        }

        static int[] Create(TokenType type = TokenType.Invalid)
        {
            var state = new int[Max];

            for (int j = Nul; j < Max; j++)
                state[j] = (int) type << 16;

            return state;
        }

        static bool[] GetIdentifierChars()
        {
            var retval = new bool[Max];
            for (int c = Nul; c < Max; c++)
                retval[c] = c >= '0' && c <= '9' ||
                            c >= 'a' && c <= 'z' ||
                            c >= 'A' && c <= 'Z' ||
                            c == '_';
            return retval;
        }

        static bool[] GetBinaryChars()
        {
            var retval = new bool[Max];
            for (int c = Nul; c < Max; c++)
                retval[c] = c >= '0' && c <= '1';
            return retval;
        }

        static bool[] GetOctalChars()
        {
            var retval = new bool[Max];
            for (int c = Nul; c < Max; c++)
                retval[c] = c >= '0' && c <= '7';
            return retval;
        }

        static bool[] GetDecimalChars()
        {
            var retval = new bool[Max];
            for (int c = Nul; c < Max; c++)
                retval[c] = c >= '0' && c <= '9';
            return retval;
        }

        static bool[] GetHexadecimalChars()
        {
            var retval = new bool[Max];
            for (int c = Nul; c < Max; c++)
                retval[c] = c >= '0' && c <= '9' ||
                            c >= 'a' && c <= 'f' ||
                            c >= 'A' && c <= 'F';
            return retval;
        }

        static bool[] GetWhitespaceChars()
        {
            var retval = new bool[Max];
            retval['\n'] = true;
            retval['\r'] = true;
            retval['\t'] = true;
            retval[' '] = true;
            return retval;
        }
    }
}