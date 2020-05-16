using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public class IsOp : Expression, ITypeReference
    {
        public Expression Operand;
        public DataType TestType;

        readonly DataType _boolType;
        public override DataType ReturnType => _boolType;

        public IsOp(Source src, Expression arg, DataType type, DataType boolType)
            : base(src)
        {
            Operand = arg;
            TestType = type;
            _boolType = boolType;
        }

        public override ExpressionType ExpressionType => ExpressionType.IsOp;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.AppendWhen(u.IsObject(), "(");
            Operand.Disassemble(sb, ExpressionUsage.Operand);
            sb.Append(" is ");
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
            return new IsOp(Source, Operand.CopyExpression(state), state.GetType(TestType), state.GetType(_boolType));
        }

        public DataType ReferencedType => TestType;
    }
}