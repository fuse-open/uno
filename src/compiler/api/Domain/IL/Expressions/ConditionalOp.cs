using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public class ConditionalOp : Expression
    {
        public Expression Condition, True, False;

        public override DataType ReturnType => True.ReturnType;

        public ConditionalOp(Source src, Expression cond, Expression onTrue, Expression onFalse)
            : base(src)
        {
            Condition = cond;
            True = onTrue;
            False = onFalse;
        }

        public override ExpressionType ExpressionType => ExpressionType.ConditionalOp;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.AppendWhen(u >= ExpressionUsage.Operand, "(");
            Condition.Disassemble(sb, ExpressionUsage.Operand);
            sb.Append(" ? ");
            True.Disassemble(sb, ExpressionUsage.Operand);
            sb.Append(" : ");
            False.Disassemble(sb, ExpressionUsage.Operand);
            sb.AppendWhen(u >= ExpressionUsage.Operand, ")");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.Begin(ref Condition, ExpressionUsage.Operand);
            Condition.Visit(p, ExpressionUsage.Operand);
            p.End(ref Condition, ExpressionUsage.Operand);

            p.Begin(ref True, ExpressionUsage.Operand);
            True.Visit(p, ExpressionUsage.Operand);
            p.End(ref True, ExpressionUsage.Operand);

            p.Begin(ref False, ExpressionUsage.Operand);
            False.Visit(p, ExpressionUsage.Operand);
            p.End(ref False, ExpressionUsage.Operand);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new ConditionalOp(Source, Condition.CopyExpression(state), True.CopyExpression(state), False.CopyExpression(state));
        }
    }
}