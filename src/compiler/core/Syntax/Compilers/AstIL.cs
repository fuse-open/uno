using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    class AstIL : AstExpression
    {
        public readonly Expression Expression;

        public AstIL(Expression e)
            : base(e.Source)
        {
            Expression = e;
        }

        public override AstExpressionType ExpressionType => AstExpressionType.Undef;
    }
}