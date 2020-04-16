using Uno.Math;

namespace Uno.Time.Calendars
{
    internal class GregorianYearMonthDayCalculator : YearMonthDayCalculator
    {
        // We precompute useful values for each month between these years, as we anticipate most
        // dates will be in this range.
        private const int FirstOptimizedYear = 1900;
        private const int LastOptimizedYear = 2100;

        private static readonly long[] MonthStartTicks = new long[(LastOptimizedYear + 1 - FirstOptimizedYear) * 12 + 1];
        private static readonly int[] MonthLengths = new int[(LastOptimizedYear + 1 - FirstOptimizedYear) * 12 + 1];
        private static readonly long[] YearStartTicks = new long[LastOptimizedYear + 1 - FirstOptimizedYear];
        private static readonly int[] YearStartDays = new int[LastOptimizedYear + 1 - FirstOptimizedYear];

        private const int DaysFrom0000To1970 = 719527;
        private const long AverageTicksPerGregorianYear = (long)(365.2425 * Constants.TicksPerStandardDay);

        private static readonly int[] MinDaysPerMonth = new [] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        private static readonly int[] MaxDaysPerMonth = new [] { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        private static readonly long[] MinTotalTicksByMonth;
        private static readonly long[] MaxTotalTicksByMonth;

        static GregorianYearMonthDayCalculator()
        {
            MinTotalTicksByMonth = new long[12];
            MaxTotalTicksByMonth = new long[12];
            long minSum = 0;
            long maxSum = 0;
            for (int i = 0; i < 11; i++)
            {
                minSum += MinDaysPerMonth[i] * Constants.TicksPerStandardDay;
                maxSum += MaxDaysPerMonth[i] * Constants.TicksPerStandardDay;
                MinTotalTicksByMonth[i + 1] = minSum;
                MaxTotalTicksByMonth[i + 1] = maxSum;
            }

            var instance = new GregorianYearMonthDayCalculator();
            for (int year = FirstOptimizedYear; year <= LastOptimizedYear; year++)
            {
                YearStartDays[year - FirstOptimizedYear] = instance.CalculateStartOfYearDays(year);
                YearStartTicks[year - FirstOptimizedYear] = YearStartDays[year - FirstOptimizedYear] * Constants.TicksPerStandardDay;
                for (int month = 1; month <= 12; month++)
                {
                    int yearMonthIndex = (year - FirstOptimizedYear) * 12 + month;
                    MonthStartTicks[yearMonthIndex] = instance.GetYearMonthTicks(year, month);
                    MonthLengths[yearMonthIndex] = instance.GetDaysInMonth(year, month);
                }
            }

        }

        internal GregorianYearMonthDayCalculator()
            : base(-27255, 31195, 12, AverageTicksPerGregorianYear,
                   -621355968000000000L, Era.BeforeCommon, Era.Common)
        {
        }

        internal override long GetStartOfYearInTicks(int year)
        {
            if (year < FirstOptimizedYear || year > LastOptimizedYear)
            {
                return base.GetStartOfYearInTicks(year);
            }
            return YearStartTicks[year - FirstOptimizedYear];
        }

        internal override Instant GetInstant(int year, int monthOfYear, int dayOfMonth)
        {
            int yearMonthIndex = (year - FirstOptimizedYear) * 12 + monthOfYear;
            if (year < FirstOptimizedYear || year > LastOptimizedYear - 1 || monthOfYear < 1 || monthOfYear > 12 || dayOfMonth < 1 ||
                dayOfMonth > MonthLengths[yearMonthIndex])
            {
                return base.GetInstant(year, monthOfYear, dayOfMonth);
            }
            // This is guaranteed not to overflow, as we've already validated the arguments
            return new Instant(MonthStartTicks[yearMonthIndex] + (dayOfMonth - 1) * Constants.TicksPerStandardDay);
        }

        protected override int CalculateStartOfYearDays(int year)
        {
            // Initial value is just temporary.
            int leapYears = year / 100;
            if (year < 0)
            {
                // Add 3 before shifting right since /4 and >>2 behave differently
                // on negative numbers. When the expression is written as
                // (year / 4) - (year / 100) + (year / 400),
                // it works for both positive and negative values, except this optimization
                // eliminates two divisions.
                leapYears = ((year + 3) >> 2) - leapYears + ((leapYears + 3) >> 2) - 1;
            }
            else
            {
                leapYears = (year >> 2) - leapYears + (leapYears >> 2);
                if (IsLeapYear(year))
                {
                    leapYears--;
                }
            }

            return year * 365 + (leapYears - DaysFrom0000To1970);
        }

        internal override bool IsLeapYear(int year)
        {
            return ((year & 3) == 0) && ((year % 100) != 0 || (year % 400) == 0);
        }

        internal override int GetCenturyOfEra(Instant instant)
        {
            return Abs(GetYear(instant)) / 100;
        }

        internal override int GetYearOfCentury(Instant instant)
        {
            return Abs(GetYear(instant)) % 100;
        }

        protected override int GetMonthOfYear(Instant instant, int year)
        {
            // Perform a binary search to get the month. To make it go even faster,
            // compare using ints instead of longs. The number of ticks per
            // year exceeds the limit of a 32-bit int's capacity, so divide by
            // 1024 * 10000. No precision is lost (except time of day) since the number of
            // ticks per day contains 1024 * 10000 as a factor. After the division,
            // the instant isn't measured in ticks, but in units of
            // (128/125) seconds.
            int i = (int)((((instant.Ticks - GetStartOfYearInTicks(year))) >> 10) / Constants.TicksPerMillisecond);

            return (IsLeapYear(year))
                       ? ((i < 182 * 84375)
                              ? ((i < 91 * 84375) ? ((i < 31 * 84375) ? 1 : (i < 60 * 84375) ? 2 : 3) : ((i < 121 * 84375) ? 4 : (i < 152 * 84375) ? 5 : 6))
                              : ((i < 274 * 84375)
                                     ? ((i < 213 * 84375) ? 7 : (i < 244 * 84375) ? 8 : 9)
                                     : ((i < 305 * 84375) ? 10 : (i < 335 * 84375) ? 11 : 12)))
                       : ((i < 181 * 84375)
                              ? ((i < 90 * 84375) ? ((i < 31 * 84375) ? 1 : (i < 59 * 84375) ? 2 : 3) : ((i < 120 * 84375) ? 4 : (i < 151 * 84375) ? 5 : 6))
                              : ((i < 273 * 84375)
                                     ? ((i < 212 * 84375) ? 7 : (i < 243 * 84375) ? 8 : 9)
                                     : ((i < 304 * 84375) ? 10 : (i < 334 * 84375) ? 11 : 12)));
        }

        internal override int GetDaysInMonth(int year, int month)
        {
            return IsLeapYear(year) ? MaxDaysPerMonth[month - 1] : MinDaysPerMonth[month - 1];
        }

        protected override long GetTicksFromStartOfYearToStartOfMonth(int year, int month)
        {
            return IsLeapYear(year) ? MaxTotalTicksByMonth[month - 1] : MinTotalTicksByMonth[month - 1];
        }

        internal override Instant GetInstant(Era era, int yearOfEra, int monthOfYear, int dayOfMonth)
        {
            int eraIndex = GetEraIndex(era);
            Preconditions.CheckArgumentRange("yearOfEra", yearOfEra, 1, GetMaxYearOfEra(eraIndex));
            return GetInstant(GetAbsoluteYear(yearOfEra, eraIndex), monthOfYear, dayOfMonth);
        }

        internal override Instant SetYear(Instant instant, int year)
        {
            int thisYear = GetYear(instant);
            int dayOfYear = GetDayOfYear(instant, thisYear);
            long tickOfDay = TimeOfDayCalculator.GetTickOfDay(instant);

            if (dayOfYear > (31 + 28))
            {
                // after Feb 28
                if (IsLeapYear(thisYear))
                {
                    // Current date is Feb 29 or later.
                    if (!IsLeapYear(year))
                    {
                        // Moving to a non-leap year, Feb 29 does not exist.
                        dayOfYear--;
                    }
                }
                else
                {
                    // Current date is Mar 01 or later.
                    if (IsLeapYear(year))
                    {
                        // Moving to a leap year, account for Feb 29.
                        dayOfYear++;
                    }
                }
            }

            long ticks = GetYearMonthDayTicks(year, 1, dayOfYear);
            return new Instant(ticks + tickOfDay);
        }

        //#region Era handling

        internal override int GetAbsoluteYear(int yearOfEra, int eraIndex)
        {
            // By now the era will have been validated; it's either 0 (BC) or 1 (AD)
            return eraIndex == 0 ? 1 - yearOfEra: yearOfEra;
        }

        internal override int GetMaxYearOfEra(int eraIndex)
        {
            // By now the era will have been validated; it's either 0 (BC) or 1 (AD)
            return eraIndex == 0 ? 1 - MinYear : MaxYear;
        }

        internal override int GetYearOfEra(Instant instant)
        {
            int year = GetYear(instant);
            return year <= 0 ? -year + 1 : year;
        }

        internal override int GetEra(Instant instant)
        {
            return instant.Ticks < TicksAtStartOfYear1 ? 0 : 1;
        }

        //#endregion
    }
}
