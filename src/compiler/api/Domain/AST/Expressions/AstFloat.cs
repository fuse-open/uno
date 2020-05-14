namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstFloat : AstExpression
    {
        public readonly float Value;

        public override AstExpressionType ExpressionType => AstExpressionType.Float;

        public AstFloat(Source src, float value)
            : base(src)
        {
            Value = value;
        }
    }
}