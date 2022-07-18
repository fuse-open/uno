using Uno.Time.Calendars;
using Uno.Collections;

namespace Uno.Time
{
    public sealed class CalendarSystem
    {
        private const string IsoName = "ISO";

        private static CalendarSystem _gregorianCalendarSystem;

        public static CalendarSystem Iso
        {
            get
            {
                if (_gregorianCalendarSystem == null)
                {
                    _gregorianCalendarSystem = new CalendarSystem(IsoName, IsoName, new GregorianYearMonthDayCalculator(), 4);
                }
                return _gregorianCalendarSystem;
            }
        }

        private readonly YearMonthDayCalculator _yearMonthDayCalculator;
        private readonly WeekYearCalculator _weekYearCalculator;
        private readonly string _id;
        private readonly string _name;
        private readonly Era[] _eras;
        private readonly int _minYear;
        private readonly int _maxYear;
        private readonly long _minTicks;
        private readonly long _maxTicks;

        private CalendarSystem(string name, YearMonthDayCalculator yearMonthDayCalculator, int minDaysInFirstWeek)
            : this(name + minDaysInFirstWeek, name, yearMonthDayCalculator, minDaysInFirstWeek)
        {
        }

        private CalendarSystem(string id, string name, YearMonthDayCalculator yearMonthDayCalculator, int minDaysInFirstWeek)
        {
            _id = id;
            _name = name;
            _yearMonthDayCalculator = yearMonthDayCalculator;
            _weekYearCalculator = new WeekYearCalculator(yearMonthDayCalculator, minDaysInFirstWeek);
            _eras = _yearMonthDayCalculator.Eras;
            _minYear = yearMonthDayCalculator.MinYear;
            _maxYear = yearMonthDayCalculator.MaxYear;
            _minTicks = yearMonthDayCalculator.GetStartOfYearInTicks(_minYear);
            _maxTicks = yearMonthDayCalculator.GetStartOfYearInTicks(_maxYear + 1) - 1;
        }

        public string Id { get { return _id; } }

        public string Name { get { return _name; } }

        public bool UsesIsoDayOfWeek { get { return true; } }

        public int MinYear { get { return _minYear; } }

        public int MaxYear { get { return _maxYear; } }

        public Era[] Eras { get { return _eras; } }

        internal long MinTicks { get { return _minTicks; } }

        internal long MaxTicks { get { return _maxTicks; } }

        internal Instant GetInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute)
        {
            Instant date = _yearMonthDayCalculator.GetInstant(year, monthOfYear, dayOfMonth);
            long timeTicks = TimeOfDayCalculator.GetTicks(hourOfDay, minuteOfHour, secondOfMinute);
            return date.PlusTicks(timeTicks);
        }

        internal Instant GetInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour)
        {
            Instant date = _yearMonthDayCalculator.GetInstant(year, monthOfYear, dayOfMonth);
            long timeTicks = TimeOfDayCalculator.GetTicks(hourOfDay, minuteOfHour);
            return date.PlusTicks(timeTicks);
        }

        internal Instant GetInstantFromWeekYearWeekAndDayOfWeek(int weekYear, int weekOfWeekYear, IsoDayOfWeek dayOfWeek)
        {
            return _weekYearCalculator.GetInstant(weekYear, weekOfWeekYear, dayOfWeek);
        }

        internal Instant GetInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute, int millisecondOfSecond, int tickOfMillisecond)
        {
            Instant date = _yearMonthDayCalculator.GetInstant(year, monthOfYear, dayOfMonth);
            long timeTicks = TimeOfDayCalculator.GetTicks(hourOfDay, minuteOfHour, secondOfMinute, millisecondOfSecond, tickOfMillisecond);
            return date.PlusTicks(timeTicks);
        }

        internal Instant GetInstant(Era era, int yearOfEra, int monthOfYear, int dayOfMonth)
        {
            return _yearMonthDayCalculator.GetInstant(era, yearOfEra, monthOfYear, dayOfMonth);
        }

        public override string ToString()
        {
            return _id;
        }

        internal IsoDayOfWeek GetIsoDayOfWeek(Instant instant)
        {
            if (!UsesIsoDayOfWeek)
            {
                throw new InvalidOperationException("Calendar " + _id + " does not use ISO days of the week");
            }
            return (IsoDayOfWeek) GetDayOfWeek(instant);
        }

        public int GetDaysInMonth(int year, int month)
        {
            return _yearMonthDayCalculator.GetDaysInMonth(year, month);
        }

        public bool IsLeapYear(int year)
        {
            return _yearMonthDayCalculator.IsLeapYear(year);
        }

        public int GetMaxMonth(int year)
        {
            return _yearMonthDayCalculator.GetMaxMonth(year);
        }

        internal int GetMaxYearOfEra(int eraIndex)
        {
            return _yearMonthDayCalculator.GetMaxYearOfEra(eraIndex);
        }

        internal int GetMinYearOfEra(int eraIndex)
        {
            return _yearMonthDayCalculator.GetMinYearOfEra(eraIndex);
        }

        internal int GetAbsoluteYear(int yearOfEra, int eraIndex)
        {
            return _yearMonthDayCalculator.GetAbsoluteYear(yearOfEra, eraIndex);
        }

        internal int GetTickOfSecond(Instant instant)
        {
            return TimeOfDayCalculator.GetTickOfSecond(instant);
        }

        internal int GetTickOfMillisecond(Instant instant)
        {
            return TimeOfDayCalculator.GetTickOfMillisecond(instant);
        }

        internal long GetTickOfDay(Instant instant)
        {
            return TimeOfDayCalculator.GetTickOfDay(instant);
        }

        internal int GetMillisecondOfSecond(Instant instant)
        {
            return TimeOfDayCalculator.GetMillisecondOfSecond(instant);
        }

        internal int GetMillisecondOfDay(Instant instant)
        {
            return TimeOfDayCalculator.GetMillisecondOfDay(instant);
        }

        internal int GetSecondOfMinute(Instant instant)
        {
            return TimeOfDayCalculator.GetSecondOfMinute(instant);
        }

        internal int GetSecondOfDay(Instant instant)
        {
            return TimeOfDayCalculator.GetSecondOfDay(instant);
        }

        internal int GetMinuteOfHour(Instant instant)
        {
            return TimeOfDayCalculator.GetMinuteOfHour(instant);
        }

        internal int GetMinuteOfDay(Instant instant)
        {
            return TimeOfDayCalculator.GetMinuteOfDay(instant);
        }

        internal int GetHourOfDay(Instant instant)
        {
            return TimeOfDayCalculator.GetHourOfDay(instant);
        }

        internal int GetHourOfHalfDay(Instant instant)
        {
            return TimeOfDayCalculator.GetHourOfHalfDay(instant);
        }

        internal int GetClockHourOfHalfDay(Instant instant)
        {
            return TimeOfDayCalculator.GetClockHourOfHalfDay(instant);
        }

        internal int GetDayOfWeek(Instant instant)
        {
            return WeekYearCalculator.GetDayOfWeek(instant);
        }

        internal int GetDayOfMonth(Instant instant)
        {
            return _yearMonthDayCalculator.GetDayOfMonth(instant);
        }

        internal int GetDayOfYear(Instant instant)
        {
            return _yearMonthDayCalculator.GetDayOfYear(instant);
        }

        internal int GetWeekOfWeekYear(Instant instant)
        {
            return _weekYearCalculator.GetWeekOfWeekYear(instant);
        }

        internal int GetWeekYear(Instant instant)
        {
            return _weekYearCalculator.GetWeekYear(instant);
        }

        internal int GetMonthOfYear(Instant instant)
        {
            return _yearMonthDayCalculator.GetMonthOfYear(instant);
        }

        internal int GetYear(Instant instant)
        {
            return _yearMonthDayCalculator.GetYear(instant);
        }

        internal int GetYearOfCentury(Instant instant)
        {
            return _yearMonthDayCalculator.GetYearOfCentury(instant);
        }

        internal int GetYearOfEra(Instant instant)
        {
            return _yearMonthDayCalculator.GetYearOfEra(instant);
        }

        internal int GetCenturyOfEra(Instant instant)
        {
            return _yearMonthDayCalculator.GetCenturyOfEra(instant);
        }

        internal int GetEra(Instant instant)
        {
            return _yearMonthDayCalculator.GetEra(instant);
        }

        internal Instant AddYears(Instant instant, long value)
        {
            int currentYear = _yearMonthDayCalculator.GetYear(instant);
            Preconditions.CheckArgumentRange("value", value, _yearMonthDayCalculator.MinYear - currentYear, _yearMonthDayCalculator.MaxYear - currentYear);
            return _yearMonthDayCalculator.SetYear(instant, (int)value + currentYear);
        }

        internal long YearDifference(Instant minuendInstant, Instant subtrahendInstant)
        {
            int minuendYear = _yearMonthDayCalculator.GetYear(minuendInstant);
            int subtrahendYear = _yearMonthDayCalculator.GetYear(subtrahendInstant);

            int diff = minuendYear - subtrahendYear;

            Instant simpleAddition = AddYears(subtrahendInstant, diff);

            if (subtrahendInstant <= minuendInstant)
            {
                return simpleAddition <= minuendInstant ? diff : diff - 1;
            }
            else
            {
                return simpleAddition >= minuendInstant ? diff : diff + 1;
            }
        }

        internal Instant AddMonths(Instant instant, long value)
        {
            if (value < int.MinValue || value > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            return _yearMonthDayCalculator.AddMonths(instant, (int)value);
        }

        internal long MonthDifference(Instant minuendInstant, Instant subtrahendInstant)
        {
            return _yearMonthDayCalculator.MonthsBetween(minuendInstant, subtrahendInstant);
        }
    }
}
