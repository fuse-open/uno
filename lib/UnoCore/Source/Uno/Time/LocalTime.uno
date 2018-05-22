using Uno.Time.Calendars;
using Uno.Time.Text;

namespace Uno.Time
{
    public struct LocalTime
    {
        public static readonly LocalTime Midnight;

        public static readonly LocalTime Noon;

        static LocalTime()
        {
            Midnight = new LocalTime(0, 0, 0);
            Noon = new LocalTime(12, 0, 0);
        }

        private readonly long _ticks;

        public LocalTime(int hour, int minute)
        {
            Preconditions.CheckArgumentRange("hour", hour, 0, Constants.HoursPerStandardDay - 1);
            Preconditions.CheckArgumentRange("minute", minute, 0, Constants.MinutesPerHour - 1);
            _ticks = unchecked(hour * Constants.TicksPerHour + minute * Constants.TicksPerMinute);
        }

        public LocalTime(int hour, int minute, int second)
        {
            Preconditions.CheckArgumentRange("hour", hour, 0, Constants.HoursPerStandardDay - 1);
            Preconditions.CheckArgumentRange("minute", minute, 0, Constants.MinutesPerHour - 1);
            Preconditions.CheckArgumentRange("second", second, 0, Constants.SecondsPerMinute - 1);
            _ticks = hour * Constants.TicksPerHour +
                    minute * Constants.TicksPerMinute +
                    second * Constants.TicksPerSecond;
        }

        public LocalTime(int hour, int minute, int second, int millisecond)
        {
            Preconditions.CheckArgumentRange("hour", hour, 0, Constants.HoursPerStandardDay - 1);
            Preconditions.CheckArgumentRange("minute", minute, 0, Constants.MinutesPerHour - 1);
            Preconditions.CheckArgumentRange("second", second, 0, Constants.SecondsPerMinute - 1);
            Preconditions.CheckArgumentRange("millisecond", millisecond, 0, Constants.MillisecondsPerSecond - 1);
            _ticks = hour * Constants.TicksPerHour +
                    minute * Constants.TicksPerMinute +
                    second * Constants.TicksPerSecond +
                    millisecond * Constants.TicksPerMillisecond;
        }

        public LocalTime(int hour, int minute, int second, int millisecond, int tickWithinMillisecond)
        {
            Preconditions.CheckArgumentRange("hour", hour, 0, Constants.HoursPerStandardDay - 1);
            Preconditions.CheckArgumentRange("minute", minute, 0, Constants.MinutesPerHour - 1);
            Preconditions.CheckArgumentRange("second", second, 0, Constants.SecondsPerMinute - 1);
            Preconditions.CheckArgumentRange("millisecond", millisecond, 0, Constants.MillisecondsPerSecond - 1);
            Preconditions.CheckArgumentRange("tickWithinMillisecond", tickWithinMillisecond, 0, Constants.TicksPerMillisecond - 1);
            _ticks = hour * Constants.TicksPerHour +
                    minute * Constants.TicksPerMinute +
                    second * Constants.TicksPerSecond +
                    millisecond * Constants.TicksPerMillisecond +
                    tickWithinMillisecond;
        }

        public LocalDateTime On(LocalDate date)
        {
            return date + this;
        }

        public static LocalTime FromHourMinuteSecondTick(int hour, int minute, int second, int tickWithinSecond)
        {
            Preconditions.CheckArgumentRange("hour", hour, 0, Constants.HoursPerStandardDay - 1);
            Preconditions.CheckArgumentRange("minute", minute, 0, Constants.MinutesPerHour - 1);
            Preconditions.CheckArgumentRange("second", second, 0, Constants.SecondsPerMinute - 1);
            Preconditions.CheckArgumentRange("tickWithinSecond", tickWithinSecond, 0, Constants.TicksPerSecond - 1);
            return new LocalTime(
                hour * Constants.TicksPerHour +
                minute * Constants.TicksPerMinute +
                second * Constants.TicksPerSecond +
                tickWithinSecond);
        }

        public static LocalTime FromTicksSinceMidnight(long ticks)
        {
            Preconditions.CheckArgumentRange("ticks", ticks, 0, Constants.TicksPerStandardDay - 1);
            return new LocalTime(ticks);
        }

        public static LocalTime FromMillisecondsSinceMidnight(int milliseconds)
        {
            Preconditions.CheckArgumentRange("milliseconds", milliseconds, 0, Constants.MillisecondsPerStandardDay - 1);
            return new LocalTime(milliseconds * Constants.TicksPerMillisecond);
        }

        public static LocalTime FromSecondsSinceMidnight(int seconds)
        {
            Preconditions.CheckArgumentRange("seconds", seconds, 0, Constants.SecondsPerStandardDay - 1);
            return new LocalTime(seconds * Constants.TicksPerSecond);
        }

        internal LocalTime(long ticks)
        {
            _ticks = ticks;
        }

        public int Hour { get { return TimeOfDayCalculator.GetHourOfDayFromTickOfDay(_ticks); } }

        public int ClockHourOfHalfDay { get { return CalendarSystem.Iso.GetClockHourOfHalfDay(new Instant(_ticks)); } }

        public int Minute { get { return TimeOfDayCalculator.GetMinuteOfHourFromTickOfDay(_ticks); } }

        public int Second { get { return TimeOfDayCalculator.GetSecondOfMinuteFromTickOfDay(_ticks); } }

        public int Millisecond { get { return TimeOfDayCalculator.GetMillisecondOfSecondFromTickOfDay(_ticks); } }

        public int TickOfSecond { get { return TimeOfDayCalculator.GetTickOfSecondFromTickOfDay(_ticks); } }

        public long TickOfDay { get { return _ticks; } }

        public LocalDateTime LocalDateTime { get { return new LocalDateTime(new Instant(_ticks)); } }

        public static LocalTime operator +(LocalTime time, Period period)
        {
            Preconditions.CheckNotNull(period, "period");
            Preconditions.CheckArgument(!period.HasDateComponent, "period", "Cannot add a period with a date component to a time");
            return (time.LocalDateTime + period).TimeOfDay;
        }

        public static LocalTime Add(LocalTime time, Period period)
        {
            return time + period;
        }

        public LocalTime Plus(Period period)
        {
            return this + period;
        }

        public static LocalTime operator -(LocalTime time, Period period)
        {
            Preconditions.CheckNotNull(period, "period");
            Preconditions.CheckArgument(!period.HasDateComponent, "period", "Cannot subtract a period with a date component from a time");
            return (time.LocalDateTime - period).TimeOfDay;
        }

        public static LocalTime Subtract(LocalTime time, Period period)
        {
            return time - period;
        }

        public LocalTime Minus(Period period)
        {
            return this - period;
        }

        public static bool operator ==(LocalTime lhs, LocalTime rhs)
        {
            return lhs._ticks == rhs._ticks;
        }

        public static bool operator !=(LocalTime lhs, LocalTime rhs)
        {
            return lhs._ticks != rhs._ticks;
        }

        public static bool operator <(LocalTime lhs, LocalTime rhs)
        {
            return lhs._ticks < rhs._ticks;
        }

        public static bool operator <=(LocalTime lhs, LocalTime rhs)
        {
            return lhs._ticks <= rhs._ticks;
        }

        public static bool operator >(LocalTime lhs, LocalTime rhs)
        {
            return lhs._ticks > rhs._ticks;
        }

        public static bool operator >=(LocalTime lhs, LocalTime rhs)
        {
            return lhs._ticks >= rhs._ticks;
        }

        public override int GetHashCode()
        {
            var ticks = _ticks;
            return ticks.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is LocalTime)
            {
                return this == (LocalTime)obj;
            }
            return false;
        }

        public bool Equals(LocalTime other)
        {
            return this == other;
        }

        public LocalTime PlusHours(long hours)
        {
            return LocalDateTime.PlusHours(hours).TimeOfDay;
        }

        public LocalTime PlusMinutes(long minutes)
        {
            return LocalDateTime.PlusMinutes(minutes).TimeOfDay;
        }

        public LocalTime PlusSeconds(long seconds)
        {
            return LocalDateTime.PlusSeconds(seconds).TimeOfDay;
        }

        public LocalTime PlusMilliseconds(long milliseconds)
        {
            return LocalDateTime.PlusMilliseconds(milliseconds).TimeOfDay;
        }

        public LocalTime PlusTicks(long ticks)
        {
            return LocalDateTime.PlusTicks(ticks).TimeOfDay;
        }

        public override string ToString()
        {
            return LocalTimePattern.GeneralIsoPattern.Format(this);
        }
    }
}
