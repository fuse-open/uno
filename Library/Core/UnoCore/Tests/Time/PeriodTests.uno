using Uno;
using Uno.Text;
using Uno.Testing;
using Uno.Time;

namespace Uno.Time.Test
{
    public class PeriodTests
    {
        public PeriodTests()
        {
            // June 19th 2010, 2:30:15am
            TestDateTime1 = new LocalDateTime(2010, 6, 19, 2, 30, 15);
            // June 19th 2010, 4:45:10am
            TestDateTime2 = new LocalDateTime(2010, 6, 19, 4, 45, 10);
            // June 19th 2010
            TestDate1 = new LocalDate(2010, 6, 19);
            // March 1st 2011
            TestDate2 = new LocalDate(2011, 3, 1);
            // March 1st 2012
            TestDate3 = new LocalDate(2012, 3, 1);
        }

        private readonly LocalDateTime TestDateTime1;
        private readonly LocalDateTime TestDateTime2;
        private readonly LocalDate TestDate1;
        private readonly LocalDate TestDate2;
        private readonly LocalDate TestDate3;

        [Test]
        public void BetweenLocalDateTimes_WithoutSpecifyingUnits_OmitsWeeks()
        {
            Period actual = Period.Between(new LocalDateTime(2012, 2, 21, 0, 0), new LocalDateTime(2012, 2, 28, 0, 0));
            Period expected = Period.FromDays(7);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDateTimes_MovingForwardWithAllFields_GivesExactResult()
        {
            Period actual = Period.Between(TestDateTime1, TestDateTime2);
            Period expected = Period.FromHours(2) + Period.FromMinutes(14) + Period.FromSeconds(55);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDateTimes_MovingBackwardWithAllFields_GivesExactResult()
        {
            Period actual = Period.Between(TestDateTime2, TestDateTime1);
            Period expected = Period.FromHours(-2) + Period.FromMinutes(-14) + Period.FromSeconds(-55);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingForwardNoLeapYears_WithExactResults()
        {
            Period actual = Period.Between(TestDate1, TestDate2);
            Period expected = Period.FromMonths(8) + Period.FromDays(10);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingForwardInLeapYear_WithExactResults()
        {
            Period actual = Period.Between(TestDate1, TestDate3);
            Period expected = Period.FromYears(1) + Period.FromMonths(8) + Period.FromDays(11);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingBackwardNoLeapYears_WithExactResults()
        {
            Period actual = Period.Between(TestDate2, TestDate1);
            Period expected = Period.FromMonths(-8) + Period.FromDays(-12);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingBackwardInLeapYear_WithExactResults()
        {
            // This is asymmetric with moving forward, because we first take off a whole year, which
            // takes us to March 1st 2011, then 8 months to take us to July 1st 2010, then 12 days
            // to take us back to June 19th. In this case, the fact that our start date is in a leap
            // year had no effect.
            Period actual = Period.Between(TestDate3, TestDate1);
            Period expected = Period.FromYears(-1) + Period.FromMonths(-8) + Period.FromDays(-12);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_AssymetricForwardAndBackward()
        {
            // February 10th 2010
            LocalDate d1 = new LocalDate(2010, 2, 10);
            // March 30th 2010
            LocalDate d2 = new LocalDate(2010, 3, 30);
            // Going forward, we go to March 10th (1 month) then March 30th (20 days)
            Assert.AreEqual(Period.FromMonths(1) + Period.FromDays(20), Period.Between(d1, d2));
            // Going backward, we go to February 28th (-1 month, day is rounded) then February 10th (-18 days)
            Assert.AreEqual(Period.FromMonths(-1) + Period.FromDays(-18), Period.Between(d2, d1));
        }

        [Test]
        public void BetweenLocalDates_EndOfMonth()
        {
            LocalDate d1 = new LocalDate(2013, 3, 31);
            LocalDate d2 = new LocalDate(2013, 4, 30);
            Assert.AreEqual(Period.FromMonths(1), Period.Between(d1, d2));
            Assert.AreEqual(Period.FromDays(-30), Period.Between(d2, d1));
        }

        [Test]
        public void BetweenLocalDates_OnLeapYear()
        {
            LocalDate d1 = new LocalDate(2012, 2, 29);
            LocalDate d2 = new LocalDate(2013, 2, 28);
            Assert.AreEqual(Period.FromYears(1), Period.Between(d1, d2));
            // Go back from February 28th 2013 to March 28th 2012, then back 28 days to February 29th 2012
            Assert.AreEqual(Period.FromMonths(-11) + Period.FromDays(-28), Period.Between(d2, d1));
        }

        [Test]
        public void BetweenLocalDates_AfterLeapYear()
        {
            LocalDate d1 = new LocalDate(2012, 3, 5);
            LocalDate d2 = new LocalDate(2013, 3, 5);
            Assert.AreEqual(Period.FromYears(1), Period.Between(d1, d2));
            Assert.AreEqual(Period.FromYears(-1), Period.Between(d2, d1));
        }

        [Test]
        public void BetweenLocalTimes_MovingForwards()
        {
            LocalTime t1 = new LocalTime(10, 0);
            LocalTime t2 = new LocalTime(15, 30, 45, 20, 5);
            Assert.AreEqual(Period.FromHours(5) + Period.FromMinutes(30) + Period.FromSeconds(45) +
                               Period.FromMilliseconds(20) + Period.FromTicks(5),
                               Period.Between(t1, t2));
        }

        [Test]
        public void BetweenLocalTimes_MovingBackwards()
        {
            LocalTime t1 = new LocalTime(15, 30, 45, 20, 5);
            LocalTime t2 = new LocalTime(10, 0);
            Assert.AreEqual(Period.FromHours(-5) + Period.FromMinutes(-30) + Period.FromSeconds(-45) +
                               Period.FromMilliseconds(-20) + Period.FromTicks(-5),
                               Period.Between(t1, t2));
        }

        [Test]
        public void Addition_WithDifferent_PeriodTypes()
        {
            Period p1 = Period.FromHours(3);
            Period p2 = Period.FromMinutes(20);
            Period sum = p1 + p2;
            Assert.AreEqual(3, sum.Hours);
            Assert.AreEqual(20, sum.Minutes);
        }

        [Test]
        public void Addition_With_IdenticalPeriodTypes()
        {
            Period p1 = Period.FromHours(3);
            Period p2 = Period.FromHours(2);
            Period sum = p1 + p2;
            Assert.AreEqual(5, sum.Hours);
        }

        [Test]
        public void Addition_DayCrossingMonthBoundary()
        {
            LocalDateTime start = new LocalDateTime(2010, 2, 20, 10, 0);
            LocalDateTime result = start + Period.FromDays(10);
            Assert.AreEqual(new LocalDateTime(2010, 3, 2, 10, 0), result);
        }

        [Test]
        public void Addition_OneYearOnLeapDay()
        {
            LocalDateTime start = new LocalDateTime(2012, 2, 29, 10, 0);
            LocalDateTime result = start + Period.FromYears(1);
            // Feb 29th becomes Feb 28th
            Assert.AreEqual(new LocalDateTime(2013, 2, 28, 10, 0), result);
        }

        [Test]
        public void Addition_FourYearsOnLeapDay()
        {
            LocalDateTime start = new LocalDateTime(2012, 2, 29, 10, 0);
            LocalDateTime result = start + Period.FromYears(4);
            // Feb 29th is still valid in 2016
            Assert.AreEqual(new LocalDateTime(2016, 2, 29, 10, 0), result);
        }

        [Test]
        public void Addition_YearMonthDay()
        {
            // One year, one month, two days
            Period period = Period.FromYears(1) + Period.FromMonths(1) + Period.FromDays(2);
            LocalDateTime start = new LocalDateTime(2007, 1, 30, 0, 0);
            // Periods are added in order, so this becomes...
            // Add one year: Jan 30th 2008
            // Add one month: Feb 29th 2008
            // Add two days: March 2nd 2008
            // If we added the days first, we'd end up with March 1st instead.
            LocalDateTime result = start + period;
            Assert.AreEqual(new LocalDateTime(2008, 3, 2, 0, 0), result);
        }

        [Test]
        public void Subtraction_WithDifferent_PeriodTypes()
        {
            Period p1 = Period.FromHours(3);
            Period p2 = Period.FromMinutes(20);
            Period sum = p1 - p2;
            Assert.AreEqual(3, sum.Hours);
            Assert.AreEqual(-20, sum.Minutes);
        }

        [Test]
        public void Subtraction_With_IdenticalPeriodTypes()
        {
            Period p1 = Period.FromHours(3);
            Period p2 = Period.FromHours(2);
            Period sum = p1 - p2;
            Assert.AreEqual(1, sum.Hours);
        }

        [Test]
        public void Equality_WhenEqual()
        {
            Assert.AreEqual(Period.FromHours(10), Period.FromHours(10));
            Assert.AreEqual(Period.FromMinutes(15), Period.FromMinutes(15));
            Assert.AreEqual(Period.FromDays(5), Period.FromDays(5));
        }

        [Test]
        public void Equality_WithDifferentPeriodTypes_OnlyConsidersValues()
        {
            Period allFields = Period.FromMinutes(1) + Period.FromHours(1) - Period.FromMinutes(1);
            Period justHours = Period.FromHours(1);
            Assert.AreEqual(allFields, justHours);
        }

        [Test]
        public void Equality_WhenUnequal()
        {
            Assert.IsFalse(Period.FromHours(10).Equals(Period.FromHours(20)));
            Assert.IsFalse(Period.FromMinutes(15).Equals(Period.FromSeconds(15)));
            Assert.IsFalse(Period.FromHours(1).Equals(Period.FromMinutes(60)));
            Assert.IsFalse(Period.FromHours(1).Equals(new object()));
            Assert.IsFalse(Period.FromHours(1).Equals(null));
            Assert.IsFalse(Period.FromHours(1).Equals((object) null));
        }

        [Test]
        public void HasTimeComponent_SingleValued()
        {
            Assert.IsTrue(Period.FromHours(1).HasTimeComponent);
            Assert.IsFalse(Period.FromDays(1).HasTimeComponent);
        }

        [Test]
        public void HasDateComponent_SingleValued()
        {
            Assert.IsFalse(Period.FromHours(1).HasDateComponent);
            Assert.IsTrue(Period.FromDays(1).HasDateComponent);
        }

        [Test]
        public void HasTimeComponent_Compound()
        {
            LocalDateTime dt1 = new LocalDateTime(2000, 1, 1, 10, 45, 00);
            LocalDateTime dt2 = new LocalDateTime(2000, 2, 4, 11, 50, 00);

            // Case 1: Entire period is date-based (no time units available)
            Assert.IsFalse(Period.Between(dt1.Date, dt2.Date).HasTimeComponent);

            // Case 2: Period contains date and time units, but time units are all zero
            Assert.IsFalse(Period.Between(dt1.Date + LocalTime.Midnight, dt2.Date + LocalTime.Midnight).HasTimeComponent);

            // Case 3: Entire period is time-based, but 0. (Same local time twice here.)
            Assert.IsFalse(Period.Between(dt1.TimeOfDay, dt1.TimeOfDay).HasTimeComponent);

            // Case 4: Period contains date and time units, and some time units are non-zero
            Assert.IsTrue(Period.Between(dt1, dt2).HasTimeComponent);

            // Case 5: Entire period is time-based, and some time units are non-zero
            Assert.IsTrue(Period.Between(dt1.TimeOfDay, dt2.TimeOfDay).HasTimeComponent);
        }

        [Test]
        public void HasDateComponent_Compound()
        {
            LocalDateTime dt1 = new LocalDateTime(2000, 1, 1, 10, 45, 00);
            LocalDateTime dt2 = new LocalDateTime(2000, 2, 4, 11, 50, 00);

            // Case 1: Entire period is time-based (no date units available)
            Assert.IsFalse(Period.Between(dt1.TimeOfDay, dt2.TimeOfDay).HasDateComponent);

            // Case 2: Period contains date and time units, but date units are all zero
            Assert.IsFalse(Period.Between(dt1, dt1.Date + dt2.TimeOfDay).HasDateComponent);

            // Case 3: Entire period is date-based, but 0. (Same local date twice here.)
            Assert.IsFalse(Period.Between(dt1.Date, dt1.Date).HasDateComponent);

            // Case 4: Period contains date and time units, and some date units are non-zero
            Assert.IsTrue(Period.Between(dt1, dt2).HasDateComponent);

            // Case 5: Entire period is date-based, and some time units are non-zero
            Assert.IsTrue(Period.Between(dt1.Date, dt2.Date).HasDateComponent);
        }

        [Test]
        public void ToDuration()
        {

            Assert.Throws<InvalidOperationException>(ToDurationInvalidWithYears);
            Assert.Throws<InvalidOperationException>(ToDurationInvalidWithMonths);
        }

        private void ToDurationInvalidWithYears()
        {
            Period.FromYears(1).ToDuration();
        }

        private void ToDurationInvalidWithMonths()
        {
            Period.FromMonths(1).ToDuration();
        }

        [Test]
        public void ToDuration_ValidWithZeroValuesInMonthYearUnits()
        {
            Period period = Period.FromMonths(1) + Period.FromYears(1);
            period = period - period + Period.FromDays(1);
            Assert.IsFalse(period.HasTimeComponent);
            Assert.AreEqual(864000000000, period.ToDuration().Ticks);
            //Assert.AreEqual(Duration.OneStandardDay, period.ToDuration());
        }

        [Test]
        [Ignore]
        public void ToDuration_Overflow()
        {
            Assert.Throws<OverflowException>(ToDurationOverflow);
        }

        private void ToDurationOverflow()
        {
            Period.FromSeconds(long.MaxValue).ToDuration();
        }

        [Test]
        [Ignore]
        public void ToDuration_Overflow_WhenPossiblyValid()
        {
            Assert.Throws<OverflowException>(ToDurationOverflowWhenPossiblyValid);
        }

        private void ToDurationOverflowWhenPossiblyValid()
        {
            var period = Period.FromSeconds(long.MaxValue) + Period.FromMinutes(long.MinValue/60);
            period.ToDuration();
        }

        [Test]
        public void ToString()
        {
            Period period;
            period = Period.FromMonths(1) + Period.FromYears(1);
            Assert.AreEqual("P1Y1M", period.ToString());
            period += Period.FromHours(12);
            Assert.AreEqual("P1Y1MT12H", period.ToString());
            period += Period.FromSeconds(-1);
            Assert.AreEqual("P1Y1MT12H-1S", period.ToString());
        }
    }
}
