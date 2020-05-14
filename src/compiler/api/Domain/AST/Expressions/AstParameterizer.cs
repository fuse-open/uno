using System.Collections.Generic;

namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstParameterizer : AstExpression
    {
        public readonly AstExpression Base;
        public readonly IReadOnlyList<AstExpression> Arguments;

        public override AstExpressionType ExpressionType => AstExpressionType.Parameterizer;

        public AstParameterizer(AstExpression name, IReadOnlyList<AstExpression> args)
            : base(name.Source)
        {
            Base = name;
            Arguments = args;
        }
    }
}