using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.API.Domain.IL.Statements
{
    public class While : Statement
    {
        // Specifies whether or not the condition should be checked after the first
        // iteration of the body
        public readonly bool DoWhile;

        public Expression Condition;
        public Statement OptionalBody;

        public While(Source src, bool doWhile = false, Expression cond = null, Statement body = null)
            : base(src)
        {
            DoWhile = doWhile;
            Condition = cond;
            OptionalBody = body;
        }

        public override StatementType StatementType => StatementType.While;

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.VisitNullable(ref Condition);
            p.VisitNullableBody(ref OptionalBody, this);
        }

        public override Statement CopyStatement(CopyState state)
        {
            return new While(Source, DoWhile, Condition.CopyExpression(state), OptionalBody.CopyNullable(state));
        }
    };
}