namespace Uno.Diagnostics
{
    [Obsolete]
    public abstract class ProfileEvent
    {
        public abstract EventType Type { get; }
        public readonly double TimeStamp;

        protected ProfileEvent()
        {
            TimeStamp = Clock.GetSeconds() - Profile.StartTimeStamp;
        }
    }
}
