namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstPixelSampler : AstExpression
    {
        public readonly AstExpression Texture;
        public readonly AstExpression OptionalState;

        public override AstExpressionType ExpressionType => AstExpressionType.PixelSampler;

        public AstPixelSampler(Source src, AstExpression texture, AstExpression optionalState)
            : base(src)
        {
            Texture = texture;
            OptionalState = optionalState;
        }
    }
}