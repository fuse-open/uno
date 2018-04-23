using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public class StageOp : Expression
    {
        public readonly MetaStage Stage;
        public Expression Operand;

        public StageOp(Source src, MetaStage stage, Expression operand)
            : base(src)
        {
            Stage = stage;
            Operand = operand;
        }

        public override ExpressionType ExpressionType => ExpressionType.StageOp;

        public override DataType ReturnType => Operand.ReturnType;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append(Stage.ToLiteral() + "(");
            Operand.Disassemble(sb);
            sb.Append(")");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.Begin(ref Operand, u);
            Operand.Visit(p, u);
            p.End(ref Operand, u);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new StageOp(Source, Stage, Operand.CopyExpression(state));
        }
    }
}