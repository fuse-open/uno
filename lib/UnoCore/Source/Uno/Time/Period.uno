using Uno.Text;

namespace Uno.Time
{
    public class Period
    {
        private const int ValuesArraySize = 9;

        private readonly long _ticks;
        private readonly long _milliseconds;
        private readonly long _seconds;
        private readonly long _minutes;
        private readonly long _hours;
        private readonly long _days;
        private readonly long _weeks;
        private readonly long _months;
        private readonly long _years;

        public static readonly Period Zero = new Period(0, 0, 0, 0, 0, 0, 0, 0, 0);

        private Period(long[] values)
        {
            _years = values[0];
            _months = values[1];
            _weeks = values[2];
            _days = values[3];
            _hours = values[4];
            _minutes = values[5];
            _seconds = values[6];
            _milliseconds = values[7];
            _ticks = values[8];
        }

        internal Period(long years, long months, long weeks, long days, long hours, long minutes, long seconds,
            long milliseconds, long ticks)
        {
            _years = years;
            _months = months;
            _weeks = weeks;
            _days = days;
            _hours = hours;
            _minutes = minutes;
            _seconds = seconds;
            _milliseconds = milliseconds;
            _ticks = ticks;
        }

        public long Years { get { return _years; } }
        public long Months { get { return _months; } }
        public long Weeks { get { return _weeks; } }
        public long Days { get { return _days; } }
        public long Hours { get { return _hours; } }
        public long Minutes { get { return _minutes; } }
        public long Seconds { get { return _seconds; } }
        public long Milliseconds { get { return _milliseconds; } }
        public long Ticks { get { return _ticks; } }

        private long TotalStandardTicks
        {
            get
            {
                return _ticks +
                    _milliseconds * Constants.TicksPerMillisecond +
                    _seconds * Constants.TicksPerSecond +
                    _minutes * Constants.TicksPerMinute +
                    _hours * Constants.TicksPerHour +
                    _days * Constants.TicksPerStandardDay +
                    _weeks * Constants.TicksPerStandardWeek;
            }
        }

        public static Period FromYears(long years)
        {
            return new Period(years, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        public static Period FromWeeks(long weeks)
        {
            return new Period(0, 0, weeks, 0, 0, 0, 0, 0, 0);
        }

        public static Period FromMonths(long months)
        {
            return new Period(0, months, 0, 0, 0, 0, 0, 0, 0);
        }

        public static Period FromDays(long days)
        {
            return new Period(0, 0, 0, days, 0, 0, 0, 0, 0);
        }

        public static Period FromHours(long hours)
        {
            return new Period(0, 0, 0, 0, hours, 0, 0, 0, 0);
        }

        public static Period FromMinutes(long minutes)
        {
            return new Period(0, 0, 0, 0, 0, minutes, 0, 0, 0);
        }

        public static Period FromSeconds(long seconds)
        {
            return new Period(0, 0, 0, 0, 0, 0, seconds, 0, 0);
        }

        public static Period FromMilliseconds(long milliseconds)
        {
            return new Period(0, 0, 0, 0, 0, 0, 0, milliseconds, 0);
        }

        public static Period FromTicks(long ticks)
        {
            return new Period(0, 0, 0, 0, 0, 0, 0, 0, ticks);
        }

        public bool HasTimeComponent
        {
            get
            {
                return _hours != 0 || _minutes != 0 || _seconds != 0 || _milliseconds != 0 || _ticks != 0;
            }
        }

        public bool HasDateComponent
        {
            get
            {
                return _years != 0 || _months != 0 || _weeks != 0 || _days != 0;
            }
        }

        internal Instant AddTo(Instant instant, CalendarSystem calendar, int scalar)
        {
            Preconditions.CheckNotNull(calendar, "calendar");
            Instant result = instant;
            if (_years != 0)
                result = calendar.AddYears(instant, _years * scalar);
            if (_months != 0)
                result = calendar.AddMonths(result, _months * scalar);
            if (_weeks != 0)
                result = result.PlusTicks(_weeks * scalar * Constants.TicksPerStandardWeek);
            if (_days != 0)
                result = result.PlusTicks(_days * scalar * Constants.TicksPerStandardDay);
            if (_hours != 0)
                result = result.PlusTicks(_hours * scalar * Constants.TicksPerHour);
            if (_minutes != 0)
                result = result.PlusTicks(_minutes * scalar * Constants.TicksPerMinute);
            if (_seconds != 0)
                result = result.PlusTicks(_seconds * scalar * Constants.TicksPerSecond);
            if (_milliseconds != 0)
                result = result.PlusTicks(_milliseconds * scalar * Constants.TicksPerMillisecond);
            if (_ticks != 0)
                result = result.PlusTicks(_ticks * scalar);
            return result;
        }

        public Instant Add(Instant instant, long ticks)
        {
            long newTicks = instant.Ticks + ticks;
            if (newTicks < instant.Ticks)
            {
                throw new OverflowException("Period addition overflowed.");
            }
            return new Instant(newTicks);
        }

        public long Subtract(Instant minuendInstant, Instant subtrahendInstant, long ticksPerUnit)
        {
            return (minuendInstant.Ticks - subtrahendInstant.Ticks) / ticksPerUnit;
        }

        public static Period Between(LocalDate start, LocalDate end)
        {
            return Between(start.AtMidnight(), end.AtMidnight());
        }

        public static Period Between(LocalTime start, LocalTime end)
        {
            return Between(start.LocalDateTime, end.LocalDateTime);
        }

        public static Period Between(LocalDateTime start, LocalDateTime end)
        {
            CalendarSystem calendar = start.Calendar;
            Preconditions.CheckArgument(calendar.Equals(end.Calendar), "end", "start and end must use the same calendar system");

            Instant startInstant = start.Instant;
            Instant endInstant = end.Instant;

            if (startInstant == endInstant)
            {
                return Zero;
            }
            Instant remaining = startInstant;
            var years = calendar.YearDifference(endInstant, remaining);
            remaining = calendar.AddYears(remaining, years);

            var months = calendar.MonthDifference(endInstant, remaining);
            remaining = calendar.AddMonths(remaining, months);

            var remainingTicks = endInstant.Ticks - remaining.Ticks;
            var days = remainingTicks / Constants.TicksPerStandardDay;
            remainingTicks = remainingTicks % Constants.TicksPerStandardDay;
            var hours = remainingTicks / Constants.TicksPerHour;
            remainingTicks = remainingTicks % Constants.TicksPerHour;
            var minutes = remainingTicks / Constants.TicksPerMinute;
            remainingTicks = remainingTicks % Constants.TicksPerMinute;
            var seconds = remainingTicks / Constants.TicksPerSecond;
            remainingTicks = remainingTicks % Constants.TicksPerSecond;
            var milliseconds = remainingTicks / Constants.TicksPerMillisecond;
            remainingTicks = remainingTicks % Constants.TicksPerMillisecond;

            return new Period(years, months, 0, days, hours, minutes, seconds,
                              milliseconds, remainingTicks);
        }

        public Duration ToDuration()
        {
            if (Months != 0 || Years != 0)
            {
                throw new InvalidOperationException("Cannot construct duration of period with non-zero months or years.");
            }
            return Duration.FromTicks(TotalStandardTicks);
        }

        public static Period operator +(Period left, Period right)
        {
            Preconditions.CheckNotNull(left, "left");
            Preconditions.CheckNotNull(right, "right");
            long[] sum = left.ToArray();
            right.AddValuesTo(sum);
            return new Period(sum);
        }

        public static Period operator -(Period minuend, Period subtrahend)
        {
            Preconditions.CheckNotNull(minuend, "minuend");
            Preconditions.CheckNotNull(subtrahend, "subtrahend");
            long[] sum = minuend.ToArray();
            subtrahend.SubtractValuesFrom(sum);
            return new Period(sum);
        }

        private long[] ToArray()
        {
            long[] values = new long[ValuesArraySize];
            values[0] = _years;
            values[1] = _months;
            values[2] = _weeks;
            values[3] = _days;
            values[4] = _hours;
            values[5] = _minutes;
            values[6] = _seconds;
            values[7] = _milliseconds;
            values[8] = _ticks;
            return values;
        }

        private void AddValuesTo(long[] values)
        {
            values[0] += _years;
            values[1] += _months;
            values[2] += _weeks;
            values[3] += _days;
            values[4] += _hours;
            values[5] += _minutes;
            values[6] += _seconds;
            values[7] += _milliseconds;
            values[8] += _ticks;
        }

        private void SubtractValuesFrom(long[] values)
        {
            values[0] -= _years;
            values[1] -= _months;
            values[2] -= _weeks;
            values[3] -= _days;
            values[4] -= _hours;
            values[5] -= _minutes;
            values[6] -= _seconds;
            values[7] -= _milliseconds;
            values[8] -= _ticks;
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, _years);
            hash = HashCodeHelper.Hash(hash, _months);
            hash = HashCodeHelper.Hash(hash, _weeks);
            hash = HashCodeHelper.Hash(hash, _days);
            hash = HashCodeHelper.Hash(hash, _hours);
            hash = HashCodeHelper.Hash(hash, _minutes);
            hash = HashCodeHelper.Hash(hash, _seconds);
            hash = HashCodeHelper.Hash(hash, _milliseconds);
            hash = HashCodeHelper.Hash(hash, _ticks);
            return hash;
        }

        public override bool Equals(object other)
        {
            return Equals(other as Period);
        }

        public bool Equals(Period other)
        {
            if (other == null)
            {
                return false;
            }

            return _years == other._years &&
                _months == other._months &&
                _weeks == other._weeks &&
                _days == other._days &&
                _hours == other._hours &&
                _minutes == other._minutes &&
                _seconds == other._seconds &&
                _milliseconds == other._milliseconds &&
                _ticks == other._ticks;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("P");
            if (_years != 0) sb.Append(string.Format("{0}Y", _years));
            if (_months != 0) sb.Append(string.Format("{0}M", _months));
            if (_weeks != 0) sb.Append(string.Format("{0}W", _weeks));
            if (_days != 0) sb.Append(string.Format("{0}D", _days));
            if (HasTimeComponent) sb.Append("T");
            if (_hours != 0) sb.Append(string.Format("{0}H", _hours));
            if (_minutes != 0) sb.Append(string.Format("{0}M", _minutes));
            if (_seconds != 0) sb.Append(string.Format("{0}S", _seconds));
            if (_milliseconds != 0) sb.Append(string.Format("{0}s", _milliseconds));
            if (_ticks != 0) sb.Append(string.Format("{0}t", _ticks));
            return sb.ToString();
        }
    }
}
