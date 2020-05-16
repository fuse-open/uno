using System.Collections.Generic;

namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstInitializer : AstExpression
    {
        public readonly IReadOnlyList<AstExpression> Expressions;

        public override AstExpressionType ExpressionType => AstExpressionType.Initializer;

        public AstInitializer(IReadOnlyList<AstExpression> expressions)
            : base(expressions[0].Source)
        {
            Expressions = expressions;
        }
    }
}