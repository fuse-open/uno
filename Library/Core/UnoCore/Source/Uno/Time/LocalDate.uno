using Uno.Time.Calendars;
using Uno.Time.Text;

namespace Uno.Time
{
    public struct LocalDate
    {
        private readonly LocalDateTime _localTime;

        public LocalDate(int year, int month, int day)
            : this(year, month, day, CalendarSystem.Iso)
        {
        }

        public LocalDate(int year, int month, int day, CalendarSystem calendar)
            : this(new LocalDateTime(year, month, day, 0, 0, calendar))
        {
        }

        public LocalDate(Era era, int yearOfEra, int month, int day)
            : this(era, yearOfEra, month, day, CalendarSystem.Iso)
        {
        }

        public LocalDate(Era era, int yearOfEra, int month, int day, CalendarSystem calendar)
            : this(new LocalDateTime(Preconditions.CheckNotNull(calendar, "calendar").GetInstant(era, yearOfEra, month, day), calendar))
        {
        }

        internal LocalDate(LocalDateTime localTime)
        {
            _localTime = localTime;
        }

        public CalendarSystem Calendar { get { return _localTime.Calendar; } }

        public int Year { get { return _localTime.Year; } }

        public int Month { get { return _localTime.Month; } }

        public int Day { get { return _localTime.Day; } }

        public IsoDayOfWeek IsoDayOfWeek { get { return _localTime.IsoDayOfWeek; } }

        public int DayOfWeek { get { return _localTime.DayOfWeek; } }

        public int WeekYear { get { return _localTime.WeekYear; } }

        public int WeekOfWeekYear { get { return _localTime.WeekOfWeekYear; } }

        public int YearOfCentury { get { return _localTime.YearOfCentury; } }

        public int YearOfEra { get { return _localTime.YearOfEra; } }

        public Era Era { get { return _localTime.Era; } }

        public int DayOfYear { get { return _localTime.DayOfYear; } }

        public LocalDateTime AtMidnight()
        {
            return _localTime;
        }

        public LocalDateTime At(LocalTime time)
        {
            return this + time;
        }

        public static LocalDate FromWeekYearWeekAndDay(int weekYear, int weekOfWeekYear, IsoDayOfWeek dayOfWeek)
        {
            Instant instant = CalendarSystem.Iso.GetInstantFromWeekYearWeekAndDayOfWeek(weekYear, weekOfWeekYear, dayOfWeek);
            return new LocalDate(new LocalDateTime(instant));
        }

        public static LocalDate operator +(LocalDate date, Period period)
        {
            Preconditions.CheckNotNull(period, "period");
            Preconditions.CheckArgument(!period.HasTimeComponent, "period", "Cannot add a period with a time component to a date");
            return new LocalDate(date._localTime + period);
        }

        public static LocalDate Add(LocalDate date, Period period)
        {
            return date + period;
        }

        public LocalDate Plus(Period period)
        {
            return this + period;
        }

        public static LocalDateTime operator +(LocalDate date, LocalTime time)
        {
            Instant localDateInstant = date._localTime.Instant;
            Instant localInstant = new Instant(localDateInstant.Ticks + time.TickOfDay);
            return new LocalDateTime(localInstant, date._localTime.Calendar);
        }

        public static LocalDate operator -(LocalDate date, Period period)
        {
            Preconditions.CheckNotNull(period, "period");
            Preconditions.CheckArgument(!period.HasTimeComponent, "period", "Cannot subtract a period with a time component from a date");
            return new LocalDate(date._localTime - period);
        }

        public static LocalDate Subtract(LocalDate date, Period period)
        {
            return date - period;
        }

        public LocalDate Minus(Period period)
        {
            return this - period;
        }

        public LocalDate WithCalendar(CalendarSystem calendarSystem)
        {
            return new LocalDate(_localTime.WithCalendar(calendarSystem));
        }

        public LocalDate PlusYears(int years)
        {
            return new LocalDate(_localTime.PlusYears(years));
        }

        public LocalDate PlusMonths(int months)
        {
            return new LocalDate(_localTime.PlusMonths(months));
        }

        public LocalDate PlusDays(int days)
        {
            return new LocalDate(_localTime.PlusDays(days));
        }

        public LocalDate PlusWeeks(int weeks)
        {
            return new LocalDate(_localTime.PlusWeeks(weeks));
        }

        public LocalDate Next(IsoDayOfWeek targetDayOfWeek)
        {
            return new LocalDate(_localTime.Next(targetDayOfWeek));
        }

        public LocalDate Previous(IsoDayOfWeek targetDayOfWeek)
        {
            return new LocalDate(_localTime.Previous(targetDayOfWeek));
        }

        public override int GetHashCode()
        {
            return _localTime.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is LocalDate)
            {
                return this == (LocalDate)obj;
            }
            return false;
        }

        public bool Equals(LocalDate other)
        {
            return this == other;
        }

        public static bool operator ==(LocalDate lhs, LocalDate rhs)
        {
            return lhs._localTime == rhs._localTime;
        }

        public static bool operator !=(LocalDate lhs, LocalDate rhs)
        {
            return lhs._localTime != rhs._localTime;
        }

        public static bool operator <(LocalDate lhs, LocalDate rhs)
        {
            return lhs._localTime < rhs._localTime;
        }

        public static bool operator <=(LocalDate lhs, LocalDate rhs)
        {
            return lhs._localTime <= rhs._localTime;
        }

        public static bool operator >(LocalDate lhs, LocalDate rhs)
        {
            return lhs._localTime > rhs._localTime;
        }

        public static bool operator >=(LocalDate lhs, LocalDate rhs)
        {
            return lhs._localTime >= rhs._localTime;
        }

        public override string ToString()
        {
            return LocalDatePattern.GeneralIsoPattern.Format(this);
        }

    }
}
