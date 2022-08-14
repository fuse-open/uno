using Uno;
using Uno.Text;
using Uno.Testing;
using Uno.Time;

namespace Uno.Time.Test
{
    public class LocalDateTimeTests
    {
        [Test]
        public void TimeProperties_AfterEpoch()
        {
            LocalDateTime ldt = new LocalDateTime(12345, 1, 2, 15, 48, 25, 456, 3456);
            Assert.AreEqual(15, ldt.Hour);
            Assert.AreEqual(3, ldt.ClockHourOfHalfDay);
            Assert.AreEqual(48, ldt.Minute);
            Assert.AreEqual(25, ldt.Second);
            Assert.AreEqual(456, ldt.Millisecond);
            Assert.AreEqual(4563456, ldt.TickOfSecond);
            Assert.AreEqual(15 * Constants.TicksPerHour +
                            48 * Constants.TicksPerMinute +
                            25 * Constants.TicksPerSecond +
                            4563456, ldt.TickOfDay);
        }

        [Test]
        public void TimeProperties_BeforeEpoch()
        {
            LocalDateTime ldt = new LocalDateTime(-12345, 1, 2, 15, 48, 25, 456, 3456);
            Assert.AreEqual(15, ldt.Hour);
            Assert.AreEqual(3, ldt.ClockHourOfHalfDay);
            Assert.AreEqual(48, ldt.Minute);
            Assert.AreEqual(25, ldt.Second);
            Assert.AreEqual(456, ldt.Millisecond);
            Assert.AreEqual(4563456, ldt.TickOfSecond);
            Assert.AreEqual(15 * Constants.TicksPerHour +
                            48 * Constants.TicksPerMinute +
                            25 * Constants.TicksPerSecond +
                            4563456, ldt.TickOfDay);
        }

        // Verifies that negative local instant ticks don't cause a problem with the date
        [Test]
        public void TimeOfDay_Before1970()
        {
            LocalDateTime dateTime = new LocalDateTime(1965, 11, 8, 12, 5, 23);
            LocalTime expected = new LocalTime(12, 5, 23);
            Assert.AreEqual(expected, dateTime.TimeOfDay);

            Assert.AreEqual(new LocalDateTime(1970, 1, 1, 12, 5, 23), dateTime.TimeOfDay.LocalDateTime);
        }

        [Test]
        public void TimeOfDay_After1970()
        {
            LocalDateTime dateTime = new LocalDateTime(1975, 11, 8, 12, 5, 23);
            LocalTime expected = new LocalTime(12, 5, 23);
            Assert.AreEqual(expected, dateTime.TimeOfDay);

            Assert.AreEqual(new LocalDateTime(1970, 1, 1, 12, 5, 23), dateTime.TimeOfDay.LocalDateTime);
        }

        [Test]
        public void Date_Before1970()
        {
            LocalDateTime dateTime = new LocalDateTime(1965, 11, 8, 12, 5, 23);
            LocalDate expected = new LocalDate(1965, 11, 8);
            Assert.AreEqual(expected, dateTime.Date);
        }

        // Verifies that positive local instant ticks don't cause a problem with the date
        [Test]
        public void Date_After1970()
        {
            LocalDateTime dateTime = new LocalDateTime(1975, 11, 8, 12, 5, 23);
            LocalDate expected = new LocalDate(1975, 11, 8);
            Assert.AreEqual(expected, dateTime.Date);
        }

        [Test]
        public void Next()
        {
            Assert.AreEqual(new LocalDateTime(2015, 5, 16, 13, 24), new LocalDateTime(2015, 5, 12, 13, 24).Next(IsoDayOfWeek.Saturday));
            Assert.AreEqual(new LocalDateTime(2015, 5, 19, 13, 24), new LocalDateTime(2015, 5, 12, 13, 24).Next(IsoDayOfWeek.Tuesday));
        }

        [Test]
        public void Previous()
        {
            Assert.AreEqual(new LocalDateTime(2015, 5, 9, 13, 24), new LocalDateTime(2015, 5, 12, 13, 24).Previous(IsoDayOfWeek.Saturday));
            Assert.AreEqual(new LocalDateTime(2015, 5, 5, 13, 24), new LocalDateTime(2015, 5, 12, 13, 24).Previous(IsoDayOfWeek.Tuesday));
        }

        [Test]
        public void Plus()
        {
            Assert.AreEqual(new LocalDateTime(2015, 5, 16, 13, 24), new LocalDateTime(2015, 5, 12, 13, 24).Next(IsoDayOfWeek.Saturday));
            Assert.AreEqual(new LocalDateTime(2015, 5, 19, 13, 24), new LocalDateTime(2015, 5, 12, 13, 24).Next(IsoDayOfWeek.Tuesday));

            Assert.AreEqual(new LocalDateTime(2015, 5, 9, 13, 24), new LocalDateTime(2015, 5, 12, 13, 24).Previous(IsoDayOfWeek.Saturday));
            Assert.AreEqual(new LocalDateTime(2015, 5, 5, 13, 24), new LocalDateTime(2015, 5, 12, 13, 24).Previous(IsoDayOfWeek.Tuesday));

            Assert.AreEqual(new LocalDateTime(2015, 5, 16, 13, 24), new LocalDateTime(2015, 5, 12, 13, 24).PlusDays(4));
            Assert.AreEqual(new LocalDateTime(2015, 5, 10, 13, 24), new LocalDateTime(2015, 5, 12, 13, 24).PlusDays(-2));

            Assert.AreEqual(new LocalDateTime(2015, 9, 30, 13, 24), new LocalDateTime(2015, 5, 31, 13, 24).PlusMonths(4));
            Assert.AreEqual(new LocalDateTime(2012, 2, 28, 13, 24), new LocalDateTime(2013, 2, 28, 13, 24).PlusMonths(-12));

            Assert.AreEqual(new LocalDateTime(2017, 5, 31, 13, 24), new LocalDateTime(2015, 5, 31, 13, 24).PlusYears(2));
            Assert.AreEqual(new LocalDateTime(2012, 2, 28, 13, 24), new LocalDateTime(2015, 2, 28, 13, 24).PlusYears(-3));

            Assert.AreEqual(new LocalDateTime(2015, 5, 13, 9, 24), new LocalDateTime(2015, 5, 12, 13, 24).PlusHours(20));
            Assert.AreEqual(new LocalDateTime(2015, 5, 12, 10, 24), new LocalDateTime(2015, 5, 12, 13, 24).PlusHours(-3));

            Assert.AreEqual(new LocalDateTime(2015, 5, 12, 14, 14), new LocalDateTime(2015, 5, 12, 13, 24).PlusMinutes(50));
            Assert.AreEqual(new LocalDateTime(2015, 5, 12, 12, 51), new LocalDateTime(2015, 5, 12, 13, 4).PlusMinutes(-13));
        }

        [Test]
        public void Compare()
        {
            var ldt1 = new LocalDateTime(2015, 5, 19, 22, 32, 25);
            var ldt2 = new LocalDateTime(2015, 5, 19, 22, 33, 25);
            var ldt3 = new LocalDateTime(2013, 5, 19, 22, 33, 25);

            Assert.IsTrue(ldt1 >= ldt1);
            Assert.IsFalse(ldt1 < ldt1);
            Assert.IsTrue(ldt1 <= ldt2);
            Assert.IsFalse(ldt1 < ldt3);
        }

        [Test]
        public void ToString()
        {
            Assert.AreEqual("2323-02-24T02:00:59", new LocalDateTime(2323, 2, 24, 2, 0, 59).ToString());
            Assert.AreEqual("-0242-11-02T00:00:00", new LocalDateTime(-242, 11, 02, 0, 0, 0).ToString());
        }
    }
}
