namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstInt : AstExpression
    {
        public readonly int Value;

        public override AstExpressionType ExpressionType => AstExpressionType.Int;

        public AstInt(Source src, int value)
            : base(src)
        {
            Value = value;
        }
    }
}