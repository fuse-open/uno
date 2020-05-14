using System.Collections.Generic;

namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstCall : AstExpression
    {
        public readonly AstCallType Type;
        public readonly AstExpression Base;
        public readonly IReadOnlyList<AstArgument> Arguments;

        public override AstExpressionType ExpressionType => (AstExpressionType)Type;

        public AstCall(AstCallType type, AstExpression @base, IReadOnlyList<AstArgument> args)
            : base(@base.Source)
        {
            Type = type;
            Base = @base;
            Arguments = args;
        }

        public AstCall(AstCallType type, AstExpression @base, params AstArgument[] args)
            : base(@base.Source)
        {
            Type = type;
            Base = @base;
            Arguments = args;
        }
    }
}