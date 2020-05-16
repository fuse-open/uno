using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.API.Domain.IL.Statements
{
    public class For : Statement
    {
        public Statement OptionalInitializer;
        public Expression OptionalCondition;
        public Expression OptionalIncrement;
        public Statement OptionalBody;

        public override StatementType StatementType => StatementType.For;

        public For(Source src)
            : base(src)
        {
        }

        public For(Source src, Statement init, Expression cond, Expression inc, Statement body)
            : base(src)
        {
            OptionalInitializer = init;
            OptionalCondition = cond;
            OptionalIncrement = inc;
            OptionalBody = body;
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.VisitNullable(ref OptionalInitializer);
            p.Next(this);
            p.VisitNullable(ref OptionalCondition);
            p.Next(this);
            p.VisitNullable(ref OptionalIncrement, ExpressionUsage.Statement);
            p.VisitNullableBody(ref OptionalBody, this);
        }

        public override Statement CopyStatement(CopyState state)
        {
            return new For(Source, 
                OptionalInitializer.CopyNullable(state), 
                OptionalCondition.CopyNullable(state), 
                OptionalIncrement.CopyNullable(state), 
                OptionalBody.CopyNullable(state));
        }
    };
}