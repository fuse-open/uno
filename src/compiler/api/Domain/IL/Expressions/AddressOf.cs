using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class AddressOf : Expression
    {
        public Expression Operand;
        public AddressType AddressType;

        public override Expression ActualValue => Operand.ActualValue;

        public override DataType ReturnType => Operand.ReturnType;

        public AddressOf(Expression operand, AddressType type = 0)
            : base(operand.Source)
        {
            Operand = operand;
            AddressType = type;
        }

        public override ExpressionType ExpressionType => ExpressionType.AddressOf;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.AppendWhen(u.IsObject(), "(");

            if (AddressType != 0)
                sb.Append(AddressType.ToString().ToLower() + " ");

            sb.Append("&");
            Operand.Disassemble(sb, ExpressionUsage.Operand);

            sb.AppendWhen(u.IsObject(), ")");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.Begin(ref Operand, u);
            Operand.Visit(p, u);
            p.End(ref Operand, u);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new AddressOf(Operand.CopyExpression(state), AddressType);
        }
    }
}