namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstVertexAttribImplicit : AstExpression
    {
        public readonly AstExpression VertexBuffer;
        public readonly AstExpression OptionalIndexBuffer;
        public readonly bool Normalize;

        public override AstExpressionType ExpressionType => AstExpressionType.VertexAttribImplicit;

        public AstVertexAttribImplicit(Source src, AstExpression vertexBuffer, AstExpression indexBuffer, bool normalize)
            : base(src)
        {
            VertexBuffer = vertexBuffer;
            OptionalIndexBuffer = indexBuffer;
            Normalize = normalize;
        }
    }
}