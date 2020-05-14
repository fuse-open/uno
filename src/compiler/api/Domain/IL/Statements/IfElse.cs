using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.API.Domain.IL.Statements
{
    public class IfElse : Statement
    {
        public Expression Condition;
        public Statement OptionalIfBody;
        public Statement OptionalElseBody;

        public IfElse(Source src, Expression cond, Statement ifBody = null, Statement elseBody = null)
            : base(src)
        {
            Condition = cond;
            OptionalIfBody = ifBody;
            OptionalElseBody = elseBody;
        }

        public override StatementType StatementType => StatementType.IfElse;

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.VisitNullable(ref Condition);
            p.VisitNullableBody(ref OptionalIfBody, this);
            p.VisitNullableBody(ref OptionalElseBody, this);
        }

        public override Statement CopyStatement(CopyState state)
        {
            return new IfElse(Source, Condition.CopyExpression(state), OptionalIfBody.CopyNullable(state), OptionalElseBody.CopyNullable(state));
        }
    }
}