using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public class SequenceOp : Expression
    {
        public Expression Left, Right;

        public override DataType ReturnType => Right.ReturnType;
        public override ExpressionType ExpressionType => ExpressionType.SequenceOp;

        public override Expression ActualValue
        {
            get
            {
                var actual = Right.ActualValue;
                if (!ReferenceEquals(Right, actual))
                    return new SequenceOp(Left, actual);

                return this;
            }
        }

        public SequenceOp(Expression left, Expression right)
            : base(left.Source)
        {
            Left = left;
            Right = right;
        }

        public SequenceOp(Expression left, Expression leftRight, Expression right)
            : base(left.Source)
        {
            Left = new SequenceOp(left, leftRight);
            Right = right;
        }

        void DisassembleSequenceOp(StringBuilder sb, bool parentIsSequenceOp)
        {
            sb.AppendWhen(!parentIsSequenceOp, "(");

            if (Left is SequenceOp)
                (Left as SequenceOp).DisassembleSequenceOp(sb, true);
            else
                Left.Disassemble(sb, ExpressionUsage.Statement);

            sb.Append(", ");

            if (Right is SequenceOp)
                (Right as SequenceOp).DisassembleSequenceOp(sb, true);
            else
                Right.Disassemble(sb);

            sb.AppendWhen(!parentIsSequenceOp, ")");
        }

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            DisassembleSequenceOp(sb, false);
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.Begin(ref Left, ExpressionUsage.Statement);
            Left.Visit(p, ExpressionUsage.Statement);
            p.End(ref Left, ExpressionUsage.Statement);

            p.Begin(ref Right, u);
            Right.Visit(p, u);
            p.End(ref Right, u);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new SequenceOp(Left.CopyExpression(state), Right.CopyExpression(state));
        }
    }
}