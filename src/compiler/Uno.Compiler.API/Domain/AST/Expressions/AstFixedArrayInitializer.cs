using System.Collections.Generic;

namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstFixedArrayInitializer : AstExpression
    {
        public readonly AstExpression OptionalElementType;
        public readonly AstExpression OptionalSize;
        public readonly IReadOnlyList<AstExpression> OptionalValues;

        public override AstExpressionType ExpressionType => AstExpressionType.FixedArrayInitializer;

        public AstFixedArrayInitializer(Source src, AstExpression optionalElementType, AstExpression optionalSize,
                IReadOnlyList<AstExpression> optionalValues = null)
            : base(src)
        {
            OptionalElementType = optionalElementType;
            OptionalSize = optionalSize;
            OptionalValues = optionalValues;
        }
    }
}