using System.Text;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class CallMethod : CallExpression
    {
        public Method Method;
        public Expression[] Arguments;

        public override DataType ReturnType => Method.ReturnType;

        public CallMethod(Source src, Expression obj, Method method, params Expression[] args)
            : base(src)
        {
            Object = obj;
            Method = method;
            Arguments = args;
        }

        public CallMethod(Source src, Expression obj, Method method, Expression[] args, Variable ret)
            : this(src, obj, method, args)
        {
            Storage = ret;
        }

        public override ExpressionType ExpressionType => ExpressionType.CallMethod;

        public override Function Function => Method;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            if (Object != null)
                Object.Disassemble(sb, ExpressionUsage.Object);
            else
                sb.Append(Method.DeclaringType);

            sb.Append("." + Method.Name + Method.GenericSuffix + "(");

            for (int i = 0; i < Arguments.Length; i++)
            {
                sb.CommaWhen(i > 0);
                Arguments[i].Disassemble(sb);
            }

            sb.Append(")");
            sb.AppendWhen(Storage != null, "~" + Storage);
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            if (Object != null)
            {
                p.Begin(ref Object, ExpressionUsage.Object);
                Object.Visit(p, ExpressionUsage.Object);
                p.End(ref Object, ExpressionUsage.Object);
            }

            for (int i = 0; i < Arguments.Length; i++)
            {
                p.Begin(ref Arguments[i]);
                Arguments[i].Visit(p);
                p.End(ref Arguments[i]);
            }
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new CallMethod(Source, Object.CopyNullable(state), state.GetMember(Method), Arguments.Copy(state));
        }
    }
}