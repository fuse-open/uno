using System.Text;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class LoadField : FieldExpression
    {
        public override ExpressionType ExpressionType => ExpressionType.LoadField;

        public LoadField(Source src, Expression obj, Field field)
            : base(src, obj, field)
        {
            Object = obj;
            Field = field;
        }

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            if (Object != null)
                Object.Disassemble(sb, ExpressionUsage.Object);
            else
                sb.Append(Field.DeclaringType);

            sb.Append("." + Field.Name);
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
            return new LoadField(Source, Object.CopyNullable(state), state.GetMember(Field));
        }
    }
}