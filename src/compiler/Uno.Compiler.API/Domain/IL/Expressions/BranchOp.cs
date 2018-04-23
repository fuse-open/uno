using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public class BranchOp : Expression
    {
        public BranchType BranchType;
        public Expression Left, Right;

        readonly DataType _bool;
        public override DataType ReturnType => _bool;

        public BranchOp(Source src, DataType @bool, BranchType type, Expression left, Expression right)
            : base(src)
        {
            BranchType = type;
            Left = left;
            Right = right;
            _bool = @bool;
        }

        public override ExpressionType ExpressionType => ExpressionType.BranchOp;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.AppendWhen(u >= ExpressionUsage.Operand, "(");

            Left.Disassemble(sb, ExpressionUsage.Operand);
            sb.Append(BranchType == BranchType.And ? " && " : " || ");
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
            return new BranchOp(Source, state.GetType(_bool), BranchType, Left.CopyExpression(state), Right.CopyExpression(state));
        }
    }
}