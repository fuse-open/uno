namespace Uno.Diagnostics
{
    [Obsolete]
    public class EnterEvent : ProfileEvent
    {
        public readonly int Id;
        public override EventType Type { get { return EventType.Enter; } }

        public EnterEvent(int id)
        {
            Id = id;
        }
    }
}
