using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.API.Domain.IL.Statements
{
    public class Switch : Statement
    {
        public Expression ControlVariable;
        public readonly SwitchCase[] Cases;

        public Switch(Source src, Expression controlVariable, params SwitchCase[] cases)
            : base(src)
        {
            ControlVariable = controlVariable;
            Cases = cases;
        }

        public override StatementType StatementType => StatementType.Switch;

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.VisitNullable(ref ControlVariable);

            foreach (var c in Cases)
            {
                p.Next(this);
                c.Scope.Visit(p);
            }
        }

        public override Statement CopyStatement(CopyState state)
        {
            return new Switch(Source, ControlVariable.CopyExpression(state), Cases.Copy(state));
        }
    }
}