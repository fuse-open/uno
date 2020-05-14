using System.Text;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class StoreField : FieldExpression
    {
        public Expression Value;

        public override ExpressionType ExpressionType => ExpressionType.StoreField;

        public StoreField(Source src, Expression obj, Field field, Expression value)
            : base(src, obj, field)
        {
            Value = value;
        }

        public StoreField(Field field, Expression value)
            : this(Source.Unknown, null, field, value)
        {
        }

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.AppendWhen(u >= ExpressionUsage.Operand, "(");

            if (Object != null)
                Object.Disassemble(sb, ExpressionUsage.Object);
            else
                sb.Append(Field.DeclaringType);

            sb.Append("." + Field.Name + " = ");
            Value.Disassemble(sb);
            sb.AppendWhen(u >= ExpressionUsage.Operand, ")");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            if (Object != null)
            {
                p.Begin(ref Object, ExpressionUsage.Object);
                Object.Visit(p, ExpressionUsage.Object);
                p.End(ref Object, ExpressionUsage.Object);
            }

            p.Begin(ref Value);
            Value.Visit(p);
            p.End(ref Value);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new StoreField(Source, Object.CopyNullable(state), state.GetMember(Field), Value.CopyExpression(state));
        }
    }
}