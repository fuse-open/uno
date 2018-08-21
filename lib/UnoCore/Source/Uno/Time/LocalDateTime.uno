using Uno.Time.Calendars;
using Uno.Time.Text;

namespace Uno.Time
{
    public struct LocalDateTime
    {
        private readonly CalendarSystem _calendar;
        private readonly Instant _instant;

        internal LocalDateTime(Instant instant)
            : this(instant, CalendarSystem.Iso)
        {
        }

        public LocalDateTime(Instant instant, CalendarSystem calendar)
        {
            Preconditions.CheckNotNull(calendar, "calendar");
            _instant = instant;
            _calendar = calendar;
        }

        public LocalDateTime(int year, int month, int day, int hour, int minute)
            : this(year, month, day, hour, minute, CalendarSystem.Iso)
        {
        }

        public LocalDateTime(int year, int month, int day, int hour, int minute, CalendarSystem calendar)
        {
            Preconditions.CheckNotNull(calendar, "calendar");
            _instant = calendar.GetInstant(year, month, day, hour, minute);
            _calendar = calendar;
        }

        public LocalDateTime(int year, int month, int day, int hour, int minute, int second)
            : this(year, month, day, hour, minute, second, CalendarSystem.Iso)
        {
        }

        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, CalendarSystem calendar)
        {
            Preconditions.CheckNotNull(calendar, "calendar");
            _instant = calendar.GetInstant(year, month, day, hour, minute, second);
            _calendar = calendar;
        }

        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
            : this(year, month, day, hour, minute, second, millisecond, 0, CalendarSystem.Iso)
        {
        }

        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, CalendarSystem calendar)
            : this(year, month, day, hour, minute, second, millisecond, 0, calendar)
        {
        }

        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int tickWithinMillisecond)
            : this(year, month, day, hour, minute, second, millisecond, tickWithinMillisecond, CalendarSystem.Iso)
        {
        }

        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int tickWithinMillisecond, CalendarSystem calendar)
        {
            Preconditions.CheckNotNull(calendar, "calendar");
            _instant = calendar.GetInstant(year, month, day, hour, minute, second, millisecond, tickWithinMillisecond);
            _calendar = calendar;
        }

        internal Instant Instant { get { return _instant; } }

        public CalendarSystem Calendar
        {
            get { return _calendar ?? CalendarSystem.Iso; }
        }

        public int CenturyOfEra { get { return Calendar.GetCenturyOfEra(_instant); } }

        public int Year { get { return Calendar.GetYear(_instant); } }

        public int YearOfCentury { get { return Calendar.GetYearOfCentury(_instant); } }

        public int YearOfEra { get { return Calendar.GetYearOfEra(_instant); } }

        public Era Era { get { return Calendar.Eras[Calendar.GetEra(_instant)]; } }

        public int WeekYear { get { return Calendar.GetWeekYear(_instant); } }

        public int Month { get { return Calendar.GetMonthOfYear(_instant); } }

        public int WeekOfWeekYear { get { return Calendar.GetWeekOfWeekYear(_instant); } }

        public int DayOfYear { get { return Calendar.GetDayOfYear(_instant); } }

        public int Day { get { return Calendar.GetDayOfMonth(_instant); } }

        public IsoDayOfWeek IsoDayOfWeek { get { return Calendar.GetIsoDayOfWeek(_instant); } }

        public int DayOfWeek { get { return Calendar.GetDayOfWeek(_instant); } }

        public int Hour { get { return Calendar.GetHourOfDay(_instant); } }

        public int ClockHourOfHalfDay { get { return Calendar.GetClockHourOfHalfDay(_instant); } }

        public int Minute { get { return Calendar.GetMinuteOfHour(_instant); } }

        public int Second { get { return Calendar.GetSecondOfMinute(_instant); } }

        public int Millisecond { get { return Calendar.GetMillisecondOfSecond(_instant); } }

        public int TickOfSecond { get { return Calendar.GetTickOfSecond(_instant); } }

        public long TickOfDay { get { return Calendar.GetTickOfDay(_instant); } }

        public LocalTime TimeOfDay { get { return new LocalTime(TickOfDay); } }

        public LocalDate Date
        {
            get
            {
                long dayTicks = _instant.Ticks % Constants.TicksPerStandardDay;
                if (dayTicks < 0)
                {
                    dayTicks += Constants.TicksPerStandardDay;
                }
                return new LocalDate(new LocalDateTime(new Instant(_instant.Ticks - dayTicks), Calendar));
            }
        }

        public static bool operator ==(LocalDateTime left, LocalDateTime right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LocalDateTime left, LocalDateTime right)
        {
            return !(left == right);
        }

        public static bool operator <(LocalDateTime lhs, LocalDateTime rhs)
        {
            return lhs.Instant < rhs.Instant && Equals(lhs.Calendar, rhs.Calendar);
        }

        public static bool operator <=(LocalDateTime lhs, LocalDateTime rhs)
        {
            return lhs.Instant <= rhs.Instant && Equals(lhs.Calendar, rhs.Calendar);
        }

        public static bool operator >(LocalDateTime lhs, LocalDateTime rhs)
        {
            return lhs.Instant > rhs.Instant && Equals(lhs.Calendar, rhs.Calendar);
        }

        public static bool operator >=(LocalDateTime lhs, LocalDateTime rhs)
        {
            return lhs.Instant >= rhs.Instant && Equals(lhs.Calendar, rhs.Calendar);
        }

        public static LocalDateTime operator +(LocalDateTime localDateTime, Period period)
        {
            return localDateTime.Plus(period);
        }

        public static LocalDateTime Add(LocalDateTime localDateTime, Period period)
        {
            return localDateTime.Plus(period);
        }

        public LocalDateTime Plus(Period period)
        {
            Preconditions.CheckNotNull(period, "period");
            return new LocalDateTime(period.AddTo(_instant, Calendar, 1), Calendar);
        }

        public static LocalDateTime operator -(LocalDateTime localDateTime, Period period)
        {
            return localDateTime.Minus(period);
        }

        public static LocalDateTime Subtract(LocalDateTime localDateTime, Period period)
        {
            return localDateTime.Minus(period);
        }

        public LocalDateTime Minus(Period period)
        {
            Preconditions.CheckNotNull(period, "period");
            return new LocalDateTime(_instant, Calendar);
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, Instant);
            hash = HashCodeHelper.Hash(hash, Calendar);
            return hash;
        }

        public bool Equals(LocalDateTime other)
        {
            return _instant == other._instant && Calendar.Equals(other.Calendar);
        }

        public override bool Equals(object obj)
        {
            if (obj is LocalDateTime)
            {
                return Equals((LocalDateTime)obj);
            }
            return false;
        }

        public LocalDateTime WithCalendar(CalendarSystem calendarSystem)
        {
            Preconditions.CheckNotNull(calendarSystem, "calendarSystem");
            return new LocalDateTime(_instant, calendarSystem);
        }

        public LocalDateTime PlusYears(int years)
        {
            return new LocalDateTime(_calendar.AddYears(_instant, years), Calendar);
        }

        public LocalDateTime PlusMonths(int months)
        {
            return new LocalDateTime(_calendar.AddMonths(_instant, months), Calendar);
        }

        public LocalDateTime PlusDays(int days)
        {
            return new LocalDateTime(_instant.PlusTicks(days * Constants.TicksPerStandardDay), Calendar);
        }

        public LocalDateTime PlusWeeks(int weeks)
        {
            return new LocalDateTime(_instant.PlusTicks(weeks * Constants.TicksPerStandardWeek), Calendar);
        }

        public LocalDateTime PlusHours(long hours)
        {
            return new LocalDateTime(_instant.PlusTicks(hours * Constants.TicksPerHour), Calendar);
        }

        public LocalDateTime PlusMinutes(long minutes)
        {
            return new LocalDateTime(_instant.PlusTicks(minutes * Constants.TicksPerMinute), Calendar);
        }

        public LocalDateTime PlusSeconds(long seconds)
        {
            return new LocalDateTime(_instant.PlusTicks(seconds * Constants.TicksPerSecond), Calendar);
        }

        public LocalDateTime PlusMilliseconds(long milliseconds)
        {
            return new LocalDateTime(_instant.PlusTicks(milliseconds * Constants.TicksPerMillisecond), Calendar);
        }

        public LocalDateTime PlusTicks(long ticks)
        {
            return new LocalDateTime(_instant.PlusTicks(ticks), Calendar);
        }

        public LocalDateTime Next(IsoDayOfWeek targetDayOfWeek)
        {
            if (targetDayOfWeek == IsoDayOfWeek.None)
            {
                throw new ArgumentOutOfRangeException(nameof(targetDayOfWeek));
            }
            IsoDayOfWeek thisDay = IsoDayOfWeek;
            int difference = targetDayOfWeek - thisDay;
            if (difference <= 0)
            {
                difference += 7;
            }
            return PlusDays(difference);
        }

        public LocalDateTime Previous(IsoDayOfWeek targetDayOfWeek)
        {
            if (targetDayOfWeek == IsoDayOfWeek.None)
            {
                throw new ArgumentOutOfRangeException(nameof(targetDayOfWeek));
            }
            IsoDayOfWeek thisDay = IsoDayOfWeek;
            int difference = targetDayOfWeek - thisDay;
            if (difference >= 0)
            {
                difference -= 7;
            }
            return PlusDays(difference);
        }

        public OffsetDateTime WithOffset(Offset offset)
        {
            return new OffsetDateTime(this, offset);
        }

        public ZonedDateTime InUtc()
        {
            return new ZonedDateTime(this, Offset.Zero, DateTimeZone.Utc);
        }

        public ZonedDateTime InZoneStrictly(DateTimeZone zone)
        {
            Preconditions.CheckNotNull(zone, "zone");
            return zone.AtStrictly(this);
        }

        public override string ToString()
        {
            return LocalDateTimePattern.GeneralIsoPattern.Format(this);
        }
    }
}
