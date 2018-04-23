namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstDouble : AstExpression
    {
        public readonly double Value;

        public override AstExpressionType ExpressionType => AstExpressionType.Double;

        public AstDouble(Source src, double value)
            : base(src)
        {
            Value = value;
        }
    }
}