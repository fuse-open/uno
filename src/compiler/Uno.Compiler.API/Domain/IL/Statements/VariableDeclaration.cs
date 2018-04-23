using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL.Statements
{
    public class VariableDeclaration : Statement
    {
        public readonly Variable Variable;

        public override StatementType StatementType => StatementType.VariableDeclaration;

        public VariableDeclaration(Source src, Function func, string name, DataType dt, VariableType type = 0, Expression optionalValue = null)
            : base(src)
        {
            Variable = new Variable(src, func, name, dt, type, optionalValue);
        }

        public VariableDeclaration(Variable var)
            : base(var.Source)
        {
            Variable = var;
        }

        public VariableDeclaration(Variable var, Variable next)
            : base(var.Source)
        {
            Variable = var;
            var.Next = next;
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            for (var var = Variable; var != null; var = var.Next)
                p.VisitNullable(ref var.OptionalValue);
        }

        public override Statement CopyStatement(CopyState state)
        {
            return new VariableDeclaration(Variable.Copy(state));
        }

        public override string ToString()
        {
            return Variable.ValueType + " " + Variable.Name + (
                Variable.OptionalValue != null 
                    ? " = " + Variable.OptionalValue 
                    : null
                ) + ";";
        }
    }
}