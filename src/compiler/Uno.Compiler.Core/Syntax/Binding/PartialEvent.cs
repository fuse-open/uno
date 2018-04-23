using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public class PartialEvent : PartialMember
    {
        public override PartialExpressionType ExpressionType => PartialExpressionType.Event;

        public readonly Event Event;

        public PartialEvent(Source src, Event ev, Expression instance)
            : base(src, instance)
        {
            Event = ev;
        }

        public override string ToString()
        {
            return Event.ToString();
        }
    }
}