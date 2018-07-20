namespace Uno.Diagnostics
{
    [Obsolete]
    public class FreeEvent : ProfileEvent
    {
        public readonly int Class;
        public readonly int Id;

        public override EventType Type { get { return EventType.Free; } }

        public FreeEvent(int cls, int id)
        {
            Class = cls;
            Id = id;
        }
    }
}
