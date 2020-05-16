using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public class NullOp : Expression
    {
        public Expression Left, Right;

        public override DataType ReturnType => Left.ReturnType;

        public NullOp(Source src, Expression left, Expression right)
            : base(src)
        {
            Left = left;
            Right = right;
        }

        public override ExpressionType ExpressionType => ExpressionType.NullOp;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.AppendWhen(u >= ExpressionUsage.Operand, "(");
            Left.Disassemble(sb, ExpressionUsage.Operand);
            sb.Append(" ?? ");
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
            return new NullOp(Source, Left.CopyExpression(state), Right.CopyExpression(state));
        }
    }
}