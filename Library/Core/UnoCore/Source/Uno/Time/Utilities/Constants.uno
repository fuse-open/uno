namespace Uno.Time
{
    public static class Constants
    {
        public const long TicksPerMillisecond = 10000;
        public const long TicksPerSecond = TicksPerMillisecond * MillisecondsPerSecond;
        public const long TicksPerMinute = TicksPerSecond * SecondsPerMinute;
        public const long TicksPerHour = TicksPerMinute * MinutesPerHour;

        public const long TicksPerStandardDay = TicksPerHour * HoursPerStandardDay;

        public const long TicksPerStandardWeek = TicksPerStandardDay * DaysPerStandardWeek;

        public const int MillisecondsPerSecond = 1000;
        public const int MillisecondsPerMinute = MillisecondsPerSecond * SecondsPerMinute;
        public const int MillisecondsPerHour = MillisecondsPerMinute * MinutesPerHour;
        public const int MillisecondsPerStandardDay = MillisecondsPerHour * HoursPerStandardDay;
        public const int MillisecondsPerStandardWeek = MillisecondsPerStandardDay * DaysPerStandardWeek;

        public const int SecondsPerMinute = 60;
        public const int SecondsPerHour = SecondsPerMinute * MinutesPerHour;
        public const int SecondsPerStandardDay = SecondsPerHour * HoursPerStandardDay;
        public const int SecondsPerWeek = SecondsPerStandardDay * DaysPerStandardWeek;

        public const int MinutesPerHour = 60;
        public const int MinutesPerStandardDay = MinutesPerHour * HoursPerStandardDay;
        public const int MinutesPerStandardWeek = MinutesPerStandardDay * DaysPerStandardWeek;

        public const int HoursPerStandardDay = 24;
        public const int HoursPerStandardWeek = HoursPerStandardDay * DaysPerStandardWeek;

        public const int DaysPerStandardWeek = 7;

        public static readonly Instant UnixEpoch;

        public static readonly Instant BclEpoch;

        static Constants()
        {
            UnixEpoch = new Instant();
            BclEpoch = Instant.FromUtc(1, 1, 1, 0, 0);
        }
    }
}
