using System.Text;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class CallBinOp : Expression
    {
        public Operator Operator;
        public Expression Left, Right;

        public override ExpressionType ExpressionType => ExpressionType.CallBinOp;
        public override DataType ReturnType => Operator.ReturnType;

        public CallBinOp(Source src, Operator @operator, Expression left, Expression right)
            : base(src)
        {
            Operator = @operator;
            Left = left;
            Right = right;
        }

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.AppendWhen(u >= ExpressionUsage.Operand, "(");

            Left.Disassemble(sb, ExpressionUsage.Operand);
            sb.Append(" " + Operator.Symbol + " ");
            Right.Disassemble(sb, ExpressionUsage.Operand);

            sb.AppendWhen(u >= ExpressionUsage.Operand, ")");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.Begin(ref Left, ExpressionUsage.Operand);
            Left.Visit(p, ExpressionUsage.Operand);
            p.End(ref Left, ExpressionUsage.Operand);

            p.Begin(ref Right, ExpressionUsage.Operand);
            Right.Visit(p, ExpressionUsage.Operand);
            p.End(ref Right, ExpressionUsage.Operand);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new CallBinOp(Source, state.GetMember(Operator), Left.CopyExpression(state), Right.CopyExpression(state));
        }
    }
}