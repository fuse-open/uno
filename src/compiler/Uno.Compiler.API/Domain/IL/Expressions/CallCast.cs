using System.Text;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class CallCast : Expression, IMemberReference
    {
        public Cast Cast;
        public Expression Operand;

        public override DataType ReturnType => Cast.ReturnType;

        public CallCast(Source src, Cast cast, Expression arg)
            : base(src)
        {
            Cast = cast;
            Operand = arg;
        }

        public override ExpressionType ExpressionType => ExpressionType.CallCast;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.AppendWhen(u.IsObject(), "(");

            sb.Append("(" + Cast.ReturnType + ")");
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
            return new CallCast(Source, state.GetMember(Cast), Operand.CopyExpression(state));
        }

        public Member ReferencedMember => Cast;

        public DataType ReferencedType => Cast.DeclaringType;
    }
}