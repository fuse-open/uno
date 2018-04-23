using System.Text;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class NewDelegate : Expression
    {
        public Expression Object;
        public Method Method;
        public DelegateType DelegateType;

        public override DataType ReturnType => DelegateType;

        public NewDelegate(Source src, DelegateType type, Expression obj, Method method)
            : base(src)
        {
            Method = method;
            Object = obj;
            DelegateType = type;
        }

        public override ExpressionType ExpressionType => ExpressionType.NewDelegate;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("new " + DelegateType + "(");

            if (Object != null)
                Object.Disassemble(sb, ExpressionUsage.Object);
            else
                sb.Append(Method.DeclaringType);

            sb.Append("." + Method.Name + ")");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            if (Object != null)
            {
                p.Begin(ref Object, ExpressionUsage.Object);
                Object.Visit(p, ExpressionUsage.Object);
                p.End(ref Object, ExpressionUsage.Object);
            }
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new NewDelegate(Source, state.GetType(DelegateType), Object?.CopyExpression(state), state.GetMember(Method));
        }
    }
}