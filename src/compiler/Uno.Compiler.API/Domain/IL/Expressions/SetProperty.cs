using System.Text;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class SetProperty : CallExpression
    {
        public Property Property;
        public Expression Value;
        public readonly Expression[] Arguments;

        public override DataType ReturnType => Property.ReturnType;

        public SetProperty(Source src, Expression obj, Property property, Expression value, params Expression[] args)
            : base(src)
        {
            Object = obj;
            Property = property;
            Arguments = args;
            Value = value;
        }

        public override ExpressionType ExpressionType => ExpressionType.SetProperty;

        public override Function Function => Property.SetMethod;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.AppendWhen(Storage != null || u >= ExpressionUsage.Operand, "(");

            if (Object != null)
                Object.Disassemble(sb, ExpressionUsage.Object);
            else
                sb.Append(Property.DeclaringType);

            if (Property.Parameters.Length == 0)
                sb.Append("." + Property.Name + " = ");
            else
            {
                sb.Append("[");

                for (int i = 0; i < Arguments.Length; i++)
                {
                    sb.CommaWhen(i > 0);
                    Arguments[i].Disassemble(sb);
                }

                sb.Append("] = ");
            }

            Value.Disassemble(sb);
            sb.AppendWhen(Storage != null || u >= ExpressionUsage.Operand, ")");
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

            p.Begin(ref Value);
            Value.Visit(p);
            p.End(ref Value);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new SetProperty(Source, Object.CopyNullable(state), state.GetMember(Property), Value.CopyExpression(state), Arguments.Copy(state));
        }
    }
}