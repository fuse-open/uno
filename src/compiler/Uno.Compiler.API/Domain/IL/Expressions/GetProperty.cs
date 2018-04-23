using System.Text;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class GetProperty : CallExpression
    {
        public Property Property;
        public Expression[] Arguments;

        public override DataType ReturnType => Property.ReturnType;

        public GetProperty(Source src, Expression obj, Property property, params Expression[] args)
            : base(src)
        {
            Object = obj;
            Property = property;
            Arguments = args;
        }

        public override ExpressionType ExpressionType => ExpressionType.GetProperty;

        public override Function Function => Property.GetMethod;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            if (Object != null)
                Object.Disassemble(sb, ExpressionUsage.Object);
            else
                sb.Append(Property.DeclaringType);

            if (Property.Parameters.Length == 0)
                sb.Append("." + Property.Name);
            else
            {
                sb.Append("[");
                for (int i = 0; i < Arguments.Length; i++)
                {
                    sb.CommaWhen(i > 0);
                    Arguments[i].Disassemble(sb);
                }
                sb.Append("]");
            }

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
            return new GetProperty(Source, Object.CopyNullable(state), state.GetMember(Property), Arguments.Copy(state));
        }
    }
}