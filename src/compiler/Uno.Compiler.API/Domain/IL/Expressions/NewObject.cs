using System.Text;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class NewObject : Expression, IMemberReference
    {
        public Constructor Constructor;
        public Expression[] Arguments;

        public override DataType ReturnType => Constructor.DeclaringType;

        public NewObject(Source src, Constructor ctor, params Expression[] args)
            : base(src)
        {
            Constructor = ctor;
            Arguments = args;
        }

        public object[] GetArgumentValues()
        {
            var result = new object[Arguments.Length];

            for (int i = 0; i < result.Length; i++)
                result[i] = Arguments[i] is Constant 
                    ? (Arguments[i] as Constant).Value
                    : null;

            return result;
        }

        public override ExpressionType ExpressionType => ExpressionType.NewObject;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("new " + Constructor.DeclaringType + "(");

            for (int i = 0; i < Arguments.Length; i++)
            {
                sb.CommaWhen(i > 0);
                Arguments[i].Disassemble(sb);
            }

            sb.Append(")");
        }

        public override void Visit(Pass p, ExpressionUsage u = ExpressionUsage.Argument)
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
            return new NewObject(Source, state.GetMember(Constructor), Arguments.Copy(state));
        }

        public Member ReferencedMember => Constructor;

        public DataType ReferencedType => Constructor.DeclaringType;
    }
}