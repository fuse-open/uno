namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstPrev : AstExpression
    {
        public readonly uint Offset;
        public readonly AstIdentifier OptionalName;

        public override AstExpressionType ExpressionType => AstExpressionType.Prev;

        public AstPrev(Source src, uint offset = 1, AstIdentifier optionalName = null)
            : base(src)
        {
            Offset = offset;
            OptionalName = optionalName;
        }
    }
}