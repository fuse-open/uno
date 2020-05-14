using System.Text;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class CallDelegate : CallExpression
    {
        public Expression[] Arguments;

        public CallDelegate(Source src, Expression obj, params Expression[] args)
            : base(src)
        {
            Object = obj;
            Arguments = args;
        }

        public override ExpressionType ExpressionType => ExpressionType.CallDelegate;

        public DelegateType DelegateType => (DelegateType)Object.ReturnType;

        public override DataType ReturnType => DelegateType.ReturnType;

        public override Function Function => DelegateType.Function;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            Object.Disassemble(sb, ExpressionUsage.Object);
            sb.Append("(");

            for (int i = 0; i < Arguments.Length; i++)
            {
                sb.CommaWhen(i > 0);
                Arguments[i].Disassemble(sb);
            }

            sb.Append(")");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.Begin(ref Object, ExpressionUsage.Object);
            Object.Visit(p, ExpressionUsage.Object);
            p.End(ref Object, ExpressionUsage.Object);

            for (int i = 0; i < Arguments.Length; i++)
            {
                p.Begin(ref Arguments[i]);
                Arguments[i].Visit(p);
                p.End(ref Arguments[i]);
            }
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new CallDelegate(Source, Object.CopyExpression(state), Arguments.Copy(state));
        }
    }
}