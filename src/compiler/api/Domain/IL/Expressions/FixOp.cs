using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class FixOp : Expression
    {
        public FixOpType Operator;
        public Expression Operand;

        public override ExpressionType ExpressionType => ExpressionType.FixOp;

        public override DataType ReturnType => Operand.ReturnType;

        public FixOp(Source src, FixOpType op, Expression operand)
            : base(src)
        {
            Operator = op;
            Operand = operand;
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new FixOp(Source, Operator, Operand.CopyExpression(state));
        }

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.AppendWhen(u.IsObject(), "(");

            switch (Operator)
            {
                case FixOpType.IncreaseBefore: sb.Append("++"); break;
                case FixOpType.DecreaseBefore: sb.Append("--"); break;
            }

            Operand.Disassemble(sb, ExpressionUsage.Operand);

            switch (Operator)
            {
                case FixOpType.IncreaseAfter: sb.Append("++"); break;
                case FixOpType.DecreaseAfter: sb.Append("--"); break;
            }

            sb.AppendWhen(u.IsObject(), ")");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.Begin(ref Operand, ExpressionUsage.Operand);
            Operand.Visit(p, ExpressionUsage.Operand);
            p.End(ref Operand, ExpressionUsage.Operand);
        }
    }
}
