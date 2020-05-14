using System.Text;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class AddListener : CallExpression
    {
        public Event Event;
        public Expression Listener;

        public AddListener(Source src, Expression obj, Event @event, Expression listener)
            : base(src)
        {
            Object = obj;
            Event = @event;
            Listener = listener;
        }

        public override ExpressionType ExpressionType => ExpressionType.AddListener;

        public override Function Function => Event.AddMethod;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.AppendWhen(Storage != null || u >= ExpressionUsage.Operand, "(");

            if (Object != null)
                Object.Disassemble(sb, ExpressionUsage.Object);
            else
                sb.Append(Event.DeclaringType);

            sb.Append("." + Event.Name + " += ");
            Listener.Disassemble(sb);

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

            p.Begin(ref Listener);
            Listener.Visit(p);
            p.End(ref Listener);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new AddListener(Source, Object.CopyNullable(state), state.GetMember(Event), Listener.CopyExpression(state));
        }
    }
}