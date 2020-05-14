using System.Text;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class StoreLocal : Expression
    {
        public Variable Variable;
        public Expression Value;

        public override DataType ReturnType => Value.ReturnType;

        public StoreLocal(Source src, Variable var, Expression value)
            : base(src)
        {
            Value = value;
            Variable = var;
        }

        public override ExpressionType ExpressionType => ExpressionType.StoreLocal;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.AppendWhen(u >= ExpressionUsage.Operand, "(");
            sb.Append(Variable.Name + " = ");
            Value.Disassemble(sb);
            sb.AppendWhen(u >= ExpressionUsage.Operand, ")");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.Begin(ref Value);
            Value.Visit(p);
            p.End(ref Value);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new StoreLocal(Source, state.GetVariable(Variable), Value.CopyExpression(state));
        }
    }
}