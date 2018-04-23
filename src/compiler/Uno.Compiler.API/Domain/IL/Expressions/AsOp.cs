using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public class AsOp : Expression, ITypeReference
    {
        public Expression Operand;
        public DataType TestType;

        public override DataType ReturnType => TestType;

        public AsOp(Source src, Expression arg, DataType type)
            : base(src)
        {
            Operand = arg;
            TestType = type;
        }

        public override ExpressionType ExpressionType => ExpressionType.AsOp;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.AppendWhen(u.IsObject(), "(");

            Operand.Disassemble(sb, ExpressionUsage.Operand);
            sb.Append(" as ");
            sb.Append(TestType);

            sb.AppendWhen(u.IsObject(), ")");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.Begin(ref Operand, ExpressionUsage.Operand);
            Operand.Visit(p, ExpressionUsage.Operand);
            p.End(ref Operand, ExpressionUsage.Operand);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new AsOp(Source, Operand.CopyExpression(state), state.GetType(TestType));
        }

        public DataType ReferencedType => TestType;
    }
}