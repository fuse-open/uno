using Uno.Math;

namespace Uno.Time.Calendars
{
    internal abstract class YearMonthDayCalculator
    {
        private readonly int _minYear;
        internal int MinYear { get { return _minYear; } }

        private readonly int _maxYear;
        internal int MaxYear { get { return _maxYear; } }

        private readonly int _monthsInYear;

        private readonly long _averageTicksPerYear;

        private readonly long _ticksAtStartOfYear1;
        internal long TicksAtStartOfYear1 { get { return _ticksAtStartOfYear1; } }

        private readonly Era[] _eras;
        internal Era[] Eras { get { return _eras; } }

        protected YearMonthDayCalculator(int minYear, int maxYear, int monthsInYear,
            long averageTicksPerYear, long ticksAtStartOfYear1, params Era[] eras)
        {
            _minYear = minYear;
            _maxYear = maxYear;
            _monthsInYear = monthsInYear;
            _eras = Preconditions.CheckNotNull(eras, "eras");
            _averageTicksPerYear = averageTicksPerYear;
            _ticksAtStartOfYear1 = ticksAtStartOfYear1;
        }

        protected abstract long GetTicksFromStartOfYearToStartOfMonth(int year, int month);

        protected abstract int CalculateStartOfYearDays(int year);
        protected abstract int GetMonthOfYear(Instant instant, int year);
        internal abstract Instant SetYear(Instant instant, int year);
        internal abstract int GetDaysInMonth(int year, int month);
        internal abstract bool IsLeapYear(int year);

        internal virtual int GetMaxMonth(int year)
        {
            return _monthsInYear;
        }

        internal virtual Instant AddMonths(Instant instant, int months)
        {
            if (months == 0)
            {
                return instant;
            }
            // Save the time part first
            long timePart = TimeOfDayCalculator.GetTickOfDay(instant);
            // Get the year and month
            int thisYear = GetYear(instant);
            int thisMonth = GetMonthOfYear(instant, thisYear);

            // Do not refactor without careful consideration.
            // Order of calculation is important.

            int yearToUse;
            // Initially, monthToUse is zero-based
            int monthToUse = thisMonth - 1 + months;
            if (monthToUse >= 0)
            {
                yearToUse = thisYear + (monthToUse / _monthsInYear);
                monthToUse = (monthToUse % _monthsInYear) + 1;
            }
            else
            {
                yearToUse = thisYear + (monthToUse / _monthsInYear) - 1;
                monthToUse = Abs(monthToUse);
                int remMonthToUse = monthToUse % _monthsInYear;
                // Take care of the boundary condition
                if (remMonthToUse == 0)
                {
                    remMonthToUse = _monthsInYear;
                }
                monthToUse = _monthsInYear - remMonthToUse + 1;
                // Take care of the boundary condition
                if (monthToUse == 1)
                {
                    yearToUse++;
                }
            }
            // End of do not refactor.

            // Quietly force DOM to nearest sane value.
            int dayToUse = GetDayOfMonth(instant, thisYear, thisMonth);
            int maxDay = GetDaysInMonth(yearToUse, monthToUse);
            dayToUse = Min(dayToUse, maxDay);
            // Get proper date part, and return result
            long datePart = GetYearMonthDayTicks(yearToUse, monthToUse, dayToUse);
            return new Instant(datePart + timePart);
        }

        internal virtual int MonthsBetween(Instant minuendInstant, Instant subtrahendInstant)
        {
            int minuendYear = GetYear(minuendInstant);
            int subtrahendYear = GetYear(subtrahendInstant);
            int minuendMonth = GetMonthOfYear(minuendInstant);
            int subtrahendMonth = GetMonthOfYear(subtrahendInstant);

            int diff = (minuendYear - subtrahendYear) * _monthsInYear + minuendMonth - subtrahendMonth;

            // If we just add the difference in months to subtrahendInstant, what do we get?
            Instant simpleAddition = AddMonths(subtrahendInstant, diff);

            if (subtrahendInstant <= minuendInstant)
            {
                // Moving forward: if the result of the simple addition is before or equal to the minuend,
                // we're done. Otherwise, rewind a month because we've overshot.
                return simpleAddition <= minuendInstant ? diff : diff - 1;
            }
            else
            {
                // Moving backward: if the result of the simple addition (of a non-positive number)
                // is after or equal to the minuend, we're done. Otherwise, increment by a month because
                // we've overshot backwards.
                return simpleAddition >= minuendInstant ? diff : diff + 1;
            }
        }

        internal virtual long GetStartOfYearInTicks(int year)
        {
            return CalculateStartOfYearDays(year) * Constants.TicksPerStandardDay;
        }

        internal virtual int GetDayOfMonth(Instant instant)
        {
            int year = GetYear(instant);
            int month = GetMonthOfYear(instant, year);
            return GetDayOfMonth(instant, year, month);
        }

        protected int GetDayOfMonth(Instant instant, int year, int month)
        {
            long dateTicks = GetYearMonthTicks(year, month);
            long ticksWithinMonth = instant.Ticks - dateTicks;
            //return (int)(ticksWithinMonth / Constants.TicksPerStandardDay) + 1;
            return Converter.TicksToDays(ticksWithinMonth) + 1;
        }

        internal int GetDayOfYear(Instant instant)
        {
            return GetDayOfYear(instant, GetYear(instant));
        }

        internal int GetDayOfYear(Instant instant, int year)
        {
            long yearStart = GetStartOfYearInTicks(year);
            long ticksWithinYear = instant.Ticks - yearStart;
            return Converter.TicksToDays(ticksWithinYear) + 1;//(int) ((ticksWithinYear >> 14) / 52734375L) + 1;
            //return TickArithmetic.FastTicksToDays(ticksWithinYear) + 1;
            //return 0;
        }

        internal virtual int GetMonthOfYear(Instant instant)
        {
            return GetMonthOfYear(instant, GetYear(instant));
        }

        internal virtual Instant GetInstant(int year, int monthOfYear, int dayOfMonth)
        {
            Preconditions.CheckArgumentRange("year", year, MinYear, MaxYear);
            Preconditions.CheckArgumentRange("monthOfYear", monthOfYear, 1, GetMaxMonth(year));
            Preconditions.CheckArgumentRange("dayOfMonth", dayOfMonth, 1, GetDaysInMonth(year, monthOfYear));
            return new Instant(GetYearMonthDayTicks(year, monthOfYear, dayOfMonth));
        }

        internal long GetYearMonthDayTicks(int year, int month, int dayOfMonth)
        {
            long ticks = GetYearMonthTicks(year, month);
            return ticks + (dayOfMonth - 1) * Constants.TicksPerStandardDay;
        }

        internal long GetYearMonthTicks(int year, int month)
        {
            long ticks = GetStartOfYearInTicks(year);
            return ticks + GetTicksFromStartOfYearToStartOfMonth(year, month);
        }

        internal virtual Instant GetInstant(Era era, int yearOfEra, int monthOfYear, int dayOfMonth)
        {
            // Just validation
            GetEraIndex(era);
            return GetInstant(yearOfEra, monthOfYear, dayOfMonth);
        }

        protected int GetEraIndex(Era era)
        {
            Preconditions.CheckNotNull(era, "era");
            int index = -1;
            for (var i = 0; i < Eras.Length; i++)
                if (Eras[i] == era)
                    index = i;
            Preconditions.CheckArgument(index != -1, "era", "Era is not used in this calendar");
            return index;
        }

        internal int GetYear(Instant instant)
        {
            long targetTicks = instant.Ticks;
            // Get an initial estimate of the year, and the ticks value that
            // represents the start of that year. Then verify estimate and fix if
            // necessary.

            // Initial estimate uses values divided by two to avoid overflow.
            long halfTicksPerYear = _averageTicksPerYear >> 1;
            long halfTicksSinceStartOfYear1 = (targetTicks >> 1) - (_ticksAtStartOfYear1 >> 1);

            if (halfTicksSinceStartOfYear1 < 0)
            {
                // When we divide, we want to round down, not towards 0.
                halfTicksSinceStartOfYear1 += 1 - halfTicksPerYear;
            }
            int candidate = (int) (halfTicksSinceStartOfYear1 / halfTicksPerYear) + 1;

            // TODO: Convert to days at this point, and do all the rest of the calculation with days.
            // We can then remove GetTicksInYear, but make sure that everything overrides GetDaysInYear appropriately.

            // Most of the time we'll get the right year straight away, and we'll almost
            // always get it after one adjustment - but it's safer (and easier to think about)
            // if we just keep going until we know we're right.
            long candidateStart = GetStartOfYearInTicks(candidate);
            long ticksFromCandidateStartToTarget = targetTicks - candidateStart;
            if (ticksFromCandidateStartToTarget < 0)
            {
                // Our candidate year is later than we want. Keep going backwards until we've got
                // a non-negative result, which must then be correct.
                do
                {
                    candidate--;
                    ticksFromCandidateStartToTarget += GetTicksInYear(candidate);
                }
                while (ticksFromCandidateStartToTarget < 0);
                return candidate;
            }
            // Our candidate year is correct or earlier than the right one. Find out which by
            // comparing it with the length of the candidate year.
            long candidateLength = GetTicksInYear(candidate);
            while (ticksFromCandidateStartToTarget >= candidateLength)
            {
                // Our candidate year is earlier than we want, so fast forward a year,
                // removing the current candidate length from the "remaining ticks" and
                // working out the length of the new candidate.
                candidate++;
                ticksFromCandidateStartToTarget -= candidateLength;
                candidateLength = GetTicksInYear(candidate);
            }
            return candidate;
        }

        internal virtual int GetYearOfEra(Instant instant)
        {
            return GetYear(instant);
        }

        internal virtual int GetCenturyOfEra(Instant instant)
        {
            int yearOfEra = GetYearOfEra(instant);
            int zeroBasedRemainder = yearOfEra % 100;
            int zeroBasedResult = yearOfEra / 100;
            return zeroBasedRemainder == 0 ? zeroBasedResult : zeroBasedResult + 1;
        }

        internal virtual int GetYearOfCentury(Instant instant)
        {
            int yearOfEra = GetYearOfEra(instant);
            int zeroBased = yearOfEra % 100;
            return zeroBased == 0 ? 100 : zeroBased;
        }

        internal virtual int GetEra(Instant instant)
        {
            return 0;
        }

        internal virtual int GetDaysInYear(int year)
        {
            return IsLeapYear(year) ? 366 : 365;
        }

        protected virtual long GetTicksInYear(int year)
        {
            return IsLeapYear(year) ? 366 * Constants.TicksPerStandardDay : 365 * Constants.TicksPerStandardDay;
        }

        internal virtual int GetAbsoluteYear(int yearOfEra, int eraIndex)
        {
            if (yearOfEra < 1 || yearOfEra > MaxYear)
            {
                throw new ArgumentOutOfRangeException(nameof(yearOfEra));
            }
            return yearOfEra;
        }

        internal virtual int GetMinYearOfEra(int eraIndex)
        {
            return 1;
        }

        internal virtual int GetMaxYearOfEra(int eraIndex)
        {
            return MaxYear;
        }
    }
}
