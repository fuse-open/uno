namespace Uno.Diagnostics
{
    public class ExitEvent : ProfileEvent
    {
        public override EventType Type { get { return EventType.Exit; } }
    }
}
