namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstChar : AstExpression
    {
        public readonly char Value;

        public override AstExpressionType ExpressionType => AstExpressionType.Char;

        public AstChar(Source src, char value)
            : base(src)
        {
            Value = value;
        }
    }
}