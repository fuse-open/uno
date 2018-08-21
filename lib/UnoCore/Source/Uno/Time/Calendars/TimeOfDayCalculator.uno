namespace Uno.Time.Calendars
{
    internal static class TimeOfDayCalculator
    {
        internal static long GetTicks(int hourOfDay, int minuteOfHour)
        {
            Preconditions.CheckArgumentRange("hourOfDay", hourOfDay, 0, Constants.HoursPerStandardDay - 1);
            Preconditions.CheckArgumentRange("minuteOfHour", minuteOfHour, 0, Constants.MinutesPerHour - 1);
            return unchecked(hourOfDay * Constants.TicksPerHour +
                 minuteOfHour * Constants.TicksPerMinute);
        }

        internal static long GetTicks(int hourOfDay, int minuteOfHour, int secondOfMinute)
        {
            Preconditions.CheckArgumentRange("hourOfDay", hourOfDay, 0, Constants.HoursPerStandardDay - 1);
            Preconditions.CheckArgumentRange("minuteOfHour", minuteOfHour, 0, Constants.MinutesPerHour - 1);
            Preconditions.CheckArgumentRange("secondOfMinute", secondOfMinute, 0, Constants.SecondsPerMinute - 1);
            return hourOfDay * Constants.TicksPerHour +
                 minuteOfHour * Constants.TicksPerMinute +
                 secondOfMinute * Constants.TicksPerSecond;
        }

        internal static long GetTicks(int hourOfDay, int minuteOfHour, int secondOfMinute,
                                      int millisecondOfSecond, int tickOfMillisecond)
        {
            Preconditions.CheckArgumentRange("hourOfDay", hourOfDay, 0, Constants.HoursPerStandardDay - 1);
            Preconditions.CheckArgumentRange("minuteOfHour", minuteOfHour, 0, Constants.MinutesPerHour - 1);
            Preconditions.CheckArgumentRange("secondOfMinute", secondOfMinute, 0, Constants.SecondsPerMinute - 1);
            Preconditions.CheckArgumentRange("millisecondOfSecond", millisecondOfSecond, 0, Constants.MillisecondsPerSecond - 1);
            Preconditions.CheckArgumentRange("tickOfMillisecond", tickOfMillisecond, 0, Constants.TicksPerMillisecond - 1);
            return hourOfDay * Constants.TicksPerHour +
                 minuteOfHour * Constants.TicksPerMinute +
                 secondOfMinute * Constants.TicksPerSecond +
                 millisecondOfSecond * Constants.TicksPerMillisecond +
                 tickOfMillisecond;
        }

        internal static long GetTickOfDay(Instant instant)
        {
            long ticks = instant.Ticks;
            if (ticks >= 0)
            {
                int days = Converter.TicksToDays(instant.Ticks);
                return ticks - ((days * 52734375L) << 14);
            }
            else
            {
                return (864000000000 - 1) + ((ticks + 1) % 864000000000);
            }
        }

        internal static int GetTickOfSecond(Instant instant)
        {
            return GetTickOfSecondFromTickOfDay(GetTickOfDay(instant));
        }

        internal static int GetTickOfMillisecond(Instant instant)
        {
            return (int) (GetTickOfDay(instant) % Constants.TicksPerMillisecond);
        }

        internal static int GetMillisecondOfSecond(Instant instant)
        {
            return GetMillisecondOfSecondFromTickOfDay(GetTickOfDay(instant));
        }

        internal static int GetMillisecondOfDay(Instant instant)
        {
            return (int) (GetTickOfDay(instant) / Constants.TicksPerMillisecond);
        }

        internal static int GetSecondOfMinute(Instant instant)
        {
            return GetSecondOfMinuteFromTickOfDay(GetTickOfDay(instant));
        }

        internal static int GetSecondOfDay(Instant instant)
        {
            return (int) (GetTickOfDay(instant) / Constants.TicksPerSecond);
        }

        internal static int GetMinuteOfHour(Instant instant)
        {
            return GetMinuteOfHourFromTickOfDay(GetTickOfDay(instant));
        }

        internal static int GetMinuteOfDay(Instant instant)
        {
            return (int) (GetTickOfDay(instant) / Constants.TicksPerMinute);
        }

        internal static int GetHourOfDay(Instant instant)
        {
            return GetHourOfDayFromTickOfDay(GetTickOfDay(instant));
        }

        internal static int GetHourOfHalfDay(Instant instant)
        {
            return GetHourOfDay(instant) % 12;
        }

        internal static int GetClockHourOfHalfDay(Instant instant)
        {
            int hourOfHalfDay = GetHourOfHalfDay(instant);
            return hourOfHalfDay == 0 ? 12 : hourOfHalfDay;
        }

        internal static int GetHourOfDayFromTickOfDay(long tickOfDay)
        {
            return ((int) (tickOfDay >> 11)) / 17578125;
        }

        internal static int GetMinuteOfHourFromTickOfDay(long tickOfDay)
        {
            int minuteOfDay = (int) (tickOfDay / (int) Constants.TicksPerMinute);
            return minuteOfDay % Constants.MinutesPerHour;
        }

        internal static int GetSecondOfMinuteFromTickOfDay(long tickOfDay)
        {
            int secondOfDay = (int) (tickOfDay / (int) Constants.TicksPerSecond);
            return secondOfDay % Constants.SecondsPerMinute;
        }

        internal static int GetMillisecondOfSecondFromTickOfDay(long tickOfDay)
        {
            long milliSecondOfDay = (tickOfDay / (int) Constants.TicksPerMillisecond);
            return (int) (milliSecondOfDay % Constants.MillisecondsPerSecond);
        }

        internal static int GetTickOfSecondFromTickOfDay(long tickOfDay)
        {
            return (int) (tickOfDay % (int) Constants.TicksPerSecond);
        }
    }
}
