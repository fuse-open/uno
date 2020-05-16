using System.Collections.Generic;

namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstVertexAttribExplicit : AstExpression
    {
        public readonly AstExpression Type;
        public readonly IReadOnlyList<AstArgument> Arguments;

        public override AstExpressionType ExpressionType => AstExpressionType.VertexAttribExplicit;

        public AstVertexAttribExplicit(Source src, 
            AstExpression type,
            IReadOnlyList<AstArgument> args)
            : base(src)
        {
            Type = type;
            Arguments = args;
        }

        public AstVertexAttribExplicit(Source src,
            AstExpression type,
            params AstArgument[] args)
            : base(src)
        {
            Type = type;
            Arguments = args;
        }
    }
}