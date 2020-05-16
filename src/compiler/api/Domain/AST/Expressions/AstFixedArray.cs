namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstFixedArray : AstExpression
    {
        public readonly AstExpression ElementType;
        public readonly AstExpression OptionalSize;

        public override AstExpressionType ExpressionType => AstExpressionType.FixedArray;

        public AstFixedArray(Source src, AstExpression elmType, AstExpression optionalSize = null)
            : base(src)
        {
            ElementType = elmType;
            OptionalSize = optionalSize;
        }
    }
}