using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Disasm.ILView.Members
{
    public class EventItem : ILItem
    {
        public readonly Event Event;
        public override string DisplayName => Event.Name;
        public override object Object => Event;
        public override ILIcon Icon => Event.IsStatic ?
            Event.IsPublic ? ILIcon.EventStatic : ILIcon.EventStaticNonPublic :
            Event.IsPublic ? ILIcon.Event : ILIcon.EventNonPublic;

        public EventItem(Event @event)
        {
            Event = @event;
            Suffix = @event.ReturnType.ToString();

            if (@event.AddMethod != null)
                AddChild(new FunctionItem(@event.AddMethod));
            if (@event.RemoveMethod != null)
                AddChild(new FunctionItem(@event.RemoveMethod));
            if (@event.ImplicitField != null)
                AddChild(new FieldItem(@event.ImplicitField));
        }

        protected override void Disassemble(Disassembler disasm)
        {
            disasm.AppendHeader(Event);
            disasm.AppendEvent(Event);
        }
    }
}
