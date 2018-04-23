namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstULong : AstExpression
    {
        public readonly ulong Value;

        public override AstExpressionType ExpressionType => AstExpressionType.ULong;

        public AstULong(Source src, ulong value)
            : base(src)
        {
            Value = value;
        }
    }
}