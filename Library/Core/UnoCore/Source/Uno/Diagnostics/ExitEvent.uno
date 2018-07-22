namespace Uno.Diagnostics
{
    [Obsolete]
    public class ExitEvent : ProfileEvent
    {
        public override EventType Type { get { return EventType.Exit; } }
    }
}
