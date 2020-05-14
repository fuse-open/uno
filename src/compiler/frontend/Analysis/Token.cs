using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Members;
using Uno.Compiler.API.Domain.AST.Statements;

namespace Uno.Compiler.Frontend.Analysis
{
    public class Token : Source
    {
        readonly string _text;
        public readonly TokenType Type;
        public new readonly int Offset;
        public new readonly int EndOffset;
        public new string Value => File.Text.Substring(Offset, Length);
        public AstUnaryType UnaryPrefix => Tokens.UnaryPrefix[(int) Type];
        public AstUnaryType UnaryPostfix => Tokens.UnaryPostfix[(int) Type];
        public AstBinaryType BinaryType => Tokens.BinaryType[(int) Type];
        public Precedence PrecedencePlusAssociativity => Tokens.PrecedencePlusAssociativity[(int) Type];
        public Precedence Precedence => Tokens.Precedence[(int) Type];
        public ParameterModifier ParameterModifier => Tokens.ParameterModifier[(int) Type];
        public Modifiers Modifier => Tokens.Modifier[(int) Type];
        public AstValueStatementType ValueStatementType => Tokens.ValueStatementType[(int) Type];
        public AstSymbolType SymbolType => Tokens.SymbolType[(int) Type];
        public AstEmptyStatementType EmptyStatementType => Tokens.EmptyStatementType[(int) Type];
        public AstStatementModifier StatementModifier => Tokens.StatementModifier[(int) Type];
        public AstClassType ClassType => Tokens.ClassType[(int) Type];
        public string String => Tokens.String[(int) Type];
        public OperatorType UnaryOperator => Tokens.UnaryOperator[(int) Type];
        public OperatorType BinaryOperator => Tokens.BinaryOperator[(int) Type];
        public AstConstraintType ConstraintClassType => Tokens.ConstraintClassType[(int) Type];
        public BuiltinType BuiltinType => Tokens.BuiltinType[(int) Type];
        public AstConstructorCallType ConstructorCallType => Tokens.ConstructorCallType[(int) Type];

        public Token(SourceFile file, TokenType type)
            : base(file)
        {
            _text = "";
            Type = type;
        }

        public Token(SourceFile file, string text, TokenType type, int line, int column, int start, int end)
            : base(file, line, column, end - start)
        {
            _text = text;
            Type = type;
            Offset = start;
            EndOffset = end;
        }

        public Token SubToken(TokenType type, int start, int length)
        {
            return new Token(File, _text, type, Line, Column + start, Offset + start, Offset + start + length);
        }

        public bool IsValue(string value)
        {
            if (value.Length != Length)
                return false;

            for (int i = 0; i < Length; i++)
                if (_text[Offset + i] != value[i])
                    return false;

            return true;
        }

        public bool StartsWith(string value)
        {
            if (value.Length > Length)
                return false;

            for (int i = 0; i < value.Length; i++)
                if (_text[Offset + i] != value[i])
                    return false;

            return true;
        }
    }
}