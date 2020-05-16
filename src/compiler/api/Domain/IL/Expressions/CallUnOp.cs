using System.Text;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class CallUnOp : Expression, IMemberReference
    {
        public Operator Operator;
        public Expression Operand;

        public override DataType ReturnType => Operator.ReturnType;
        public override ExpressionType ExpressionType => ExpressionType.CallUnOp;
        public Member ReferencedMember => Operator;
        public DataType ReferencedType => Operator.DeclaringType;

        public CallUnOp(Source src, Operator @operator, Expression operand)
            : base(src)
        {
            Operator = @operator;
            Operand = operand;
        }

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.AppendWhen(u.IsObject(), "(");
            sb.Append(Operator.Symbol);
            Operand.Disassemble(sb, ExpressionUsage.Operand);
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
            return new CallUnOp(Source, state.GetMember(Operator), Operand.CopyExpression(state));
        }
    }
}