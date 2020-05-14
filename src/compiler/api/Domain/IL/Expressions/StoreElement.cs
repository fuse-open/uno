using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class StoreElement : Expression
    {
        public Expression Array;
        public Expression Index;
        public Expression Value;

        public StoreElement(Source src, Expression array, Expression index, Expression value)
            : base(src)
        {
            Array = array;
            Index = index;
            Value = value;
        }

        public override DataType ReturnType => Value.ReturnType;

        public override ExpressionType ExpressionType => ExpressionType.StoreElement;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.AppendWhen(u >= ExpressionUsage.Operand, "(");
            Array.Disassemble(sb, ExpressionUsage.Object);
            sb.Append("[");
            Index.Disassemble(sb);
            sb.Append("] = ");
            Value.Disassemble(sb);
            sb.AppendWhen(u >= ExpressionUsage.Operand, ")");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.Begin(ref Array, ExpressionUsage.Object);
            Array.Visit(p, ExpressionUsage.Object);
            p.End(ref Array, ExpressionUsage.Object);

            p.Begin(ref Index);
            Index.Visit(p);
            p.End(ref Index);

            p.Begin(ref Value);
            Value.Visit(p);
            p.End(ref Value);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new StoreElement(Source, Array.CopyExpression(state), Index.CopyExpression(state), Value.CopyExpression(state));
        }
    }
}