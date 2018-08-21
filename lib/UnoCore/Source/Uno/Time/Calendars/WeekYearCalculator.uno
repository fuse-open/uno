namespace Uno.Time.Calendars
{
    internal sealed class WeekYearCalculator
    {
        private readonly YearMonthDayCalculator yearMonthDayCalculator;
        private readonly int minDaysInFirstWeek;

        internal WeekYearCalculator(YearMonthDayCalculator yearMonthDayCalculator, int minDaysInFirstWeek)
        {
            this.yearMonthDayCalculator = yearMonthDayCalculator;
            this.minDaysInFirstWeek = minDaysInFirstWeek;
        }

        internal Instant GetInstant(int weekYear, int weekOfWeekYear, IsoDayOfWeek dayOfWeek)
        {
            Preconditions.CheckArgumentRange("weekYear", weekYear, yearMonthDayCalculator.MinYear, yearMonthDayCalculator.MaxYear);
            Preconditions.CheckArgumentRange("weekOfWeekYear", weekOfWeekYear, 1, GetWeeksInWeekYear(weekYear));
            // TODO: Work out what argument validation we actually want here.
            Preconditions.CheckArgumentRange("dayOfWeek", (int)dayOfWeek, 1, 7);
            long ticks = GetWeekYearTicks(weekYear)
                + (weekOfWeekYear - 1) * Constants.TicksPerStandardWeek
                + ((int) dayOfWeek - 1) * Constants.TicksPerStandardDay;
            return new Instant(ticks);
        }

        internal static int GetDayOfWeek(Instant instant)
        {
            long daysSince19700101;
            long ticks = instant.Ticks;
            if (ticks >= 0)
            {
                daysSince19700101 = Converter.TicksToDays(ticks);
            }
            else
            {
                daysSince19700101 = ((ticks >> 14) - 52734374) / 52734375;
                if (daysSince19700101 < -3)
                {
                    return 7 + (int) ((daysSince19700101 + 4) % 7);
                }
            }

            return 1 + (int) ((daysSince19700101 + 3) % 7);
        }

        internal int GetWeekOfWeekYear(Instant localInstant)
        {
            int weekYear = GetWeekYear(localInstant);
            long startOfWeekYear = GetWeekYearTicks(weekYear);
            long ticksIntoYear = localInstant.Ticks - startOfWeekYear;
            int zeroBasedWeek = (int)(ticksIntoYear / Constants.TicksPerStandardWeek);
            return zeroBasedWeek + 1;
        }

        private int GetWeeksInWeekYear(int weekYear)
        {
            long startOfWeekYear = GetWeekYearTicks(weekYear);
            long startOfCalendarYear = yearMonthDayCalculator.GetStartOfYearInTicks(weekYear);
            // The number of days gained or lost in the week year compared with the calendar year.
            // So if the week year starts on December 31st of the previous calendar year, this will be +1.
            // If the week year starts on January 2nd of this calendar year, this will be -1.

            int extraDays = (int)((startOfCalendarYear - startOfWeekYear) / Constants.TicksPerStandardDay);
            int daysInThisYear = yearMonthDayCalculator.GetDaysInYear(weekYear);

            // We can have up to "minDaysInFirstWeek - 1" days of the next year, too.
            return (daysInThisYear + extraDays + (minDaysInFirstWeek - 1)) / 7;
        }

        private long GetWeekYearTicks(int weekYear)
        {
            // Need to be slightly careful here, as the week-year can reasonably be outside the calendar year range.
            long jan1Millis = yearMonthDayCalculator.GetStartOfYearInTicks(weekYear);
            int jan1DayOfWeek = GetDayOfWeek(new Instant(jan1Millis));

            if (jan1DayOfWeek > (8 - minDaysInFirstWeek))
            {
                // First week is end of previous year because it doesn't have enough days.
                return jan1Millis + (8 - jan1DayOfWeek) * Constants.TicksPerStandardDay;
            }
            else
            {
                // First week is start of this year because it has enough days.
                return jan1Millis - (jan1DayOfWeek - 1) * Constants.TicksPerStandardDay;
            }
        }

        internal int GetWeekYear(Instant instant)
        {
            int calendarYear = yearMonthDayCalculator.GetYear(instant);
            long startOfWeekYear = GetWeekYearTicks(calendarYear);
            if (instant.Ticks < startOfWeekYear)
            {
                return calendarYear - 1;
            }
            int weeksInWeekYear = GetWeeksInWeekYear(calendarYear);
            long startOfNextCalendarYear = startOfWeekYear + weeksInWeekYear * Constants.TicksPerStandardWeek;
            return instant.Ticks < startOfNextCalendarYear ? calendarYear : calendarYear + 1;
        }
    }
}
