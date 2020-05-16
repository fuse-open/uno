namespace Uno.UX.Markup.UXIL.Expressions.Lexer
{
    public enum TokenType
    {
        EndOfFile,
        Invalid,
        Whitespace,
        Identifier,

        // Literals
        DecimalLiteral,
        FloatLiteral,
        DoubleLiteral,

        // UX literals
        PixelLiteral,
        PointLiteral,
        ColorCodeLiteral,

        // Keywords
        [Token("false")]
        False,
        [Token("this")]
        This,
        [Token("true")]
        True,

        // Punctuation
        [Token(";")]
        Semicolon,
        [Token(".")]
        Period,
        [Token(",")]
        Comma,
        [Token(":")]
        Colon,
        [Token("?")]
        QuestionMark,
        [Token("??")]
        DoubleQuestionMark,

        // Braces
        [Token("{")]
        LeftCurlyBrace,
        [Token("}")]
        RightCurlyBrace,
        [Token("(")]
        LeftParen,
        [Token(")")]
        RightParen,
        [Token("[")]
        LeftSquareBrace,
        [Token("]")]
        RightSquareBrace,

        // Unary operators
        [Token("!")]
        ExclamationMark,

        // Binary operators
        [Token("&&")]
        LogAnd,
        [Token("||")]
        LogOr,
        [Token("==")]
        Equal,
        [Token("!=")]
        NotEqual,
        [Token(">=")]
        GreaterOrEqual,
        [Token("<=")]
        LessOrEqual,
        [Token("=")]
        Assign,
        [Token("+")]
        Plus,
        [Token("-")]
        Minus,
        [Token("*")]
        Mul,
        [Token("/")]
        Div,
        [Token("%")]
        Mod,
        [Token("<")]
        LessThan,
        [Token(">")]
        GreaterThan,

        // Others
        [Token("#")]
        PreprocessorDirective,
        [Token("'")]
        CharLiteral,
        [Token("\"")]
        StringLiteral,

        Max
    }
}
