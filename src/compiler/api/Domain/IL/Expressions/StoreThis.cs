using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class StoreThis : Expression
    {
        public Expression Value;

        public override DataType ReturnType => Value.ReturnType;

        public StoreThis(Source src, Expression value)
            : base(src)
        {
            Value = value;
        }

        public override ExpressionType ExpressionType => ExpressionType.StoreThis;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.AppendWhen(u >= ExpressionUsage.Operand, "(");
            sb.Append("this = ");
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
            return new StoreThis(Source, Value.CopyExpression(state));
        }
    }
}