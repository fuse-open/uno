using Uno.Compiler;

namespace Uno.UX.Markup.UXIL.Expressions.Lexer
{
    public class Token : Source
    {
        public readonly TokenType Type;
        public new readonly int Offset;
        public new string Value => File.Text.Substring(Offset, Length);

        public Token(SourceFile file, TokenType type)
            : base(file)
        {
            Type = type;
        }

        public Token(SourceFile file, TokenType type, int line, int column, int start, int end)
            : base(file, line, column, end - start)
        {
            Type = type;
            Offset = start;
        }
    }
}
