namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstLong : AstExpression
    {
        public readonly long Value;

        public override AstExpressionType ExpressionType => AstExpressionType.Long;

        public AstLong(Source src, long value)
            : base(src)
        {
            Value = value;
        }
    }
}