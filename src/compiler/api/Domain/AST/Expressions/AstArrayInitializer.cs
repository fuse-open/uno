using System.Collections.Generic;

namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstArrayInitializer : AstExpression
    {
        public readonly IReadOnlyList<AstExpression> Values;

        public override AstExpressionType ExpressionType => AstExpressionType.ArrayInitializer;

        public AstArrayInitializer(Source src, IReadOnlyList<AstExpression> values)
            : base(src)
        {
            Values = values;
        }
    }
}