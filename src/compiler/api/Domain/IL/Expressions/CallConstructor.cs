using System.Text;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class CallConstructor : VoidExpression, IMemberReference
    {
        public Constructor Constructor;
        public Expression[] Arguments;

        public CallConstructor(Source src, Constructor ctor, params Expression[] args)
            : base(src)
        {
            Constructor = ctor;
            Arguments = args;
        }

        public override ExpressionType ExpressionType => ExpressionType.CallConstructor;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append(Constructor.DeclaringType + "." + Constructor.Name + "(");

            for (int i = 0; i < Arguments.Length; i++)
            {
                sb.CommaWhen(i > 0);
                Arguments[i].Disassemble(sb);
            }

            sb.Append(")");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            for (int i = 0; i < Arguments.Length; i++)
            {
                p.Begin(ref Arguments[i]);
                Arguments[i].Visit(p);
                p.End(ref Arguments[i]);
            }
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new CallConstructor(Source, state.GetMember(Constructor), Arguments.Copy(state));
        }

        public Member ReferencedMember => Constructor;

        public DataType ReferencedType => Constructor.DeclaringType;
    }
}