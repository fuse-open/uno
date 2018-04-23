using Uno;
using Uno.Text;
using Uno.Testing;
using Uno.Time;
using Uno.Time.Calendars;

namespace Uno.Time.Test
{
    public class ZonedDateTimeTests
    {
        [Test]
        public void OffsetProperty()
        {
            var zdt = new ZonedDateTime(Instant.FromUtc(2008, 1, 1, 0, 0), new DeviceTimeZoneMock());
            Assert.AreEqual(new OffsetDateTime(new LocalDateTime(2008, 1, 1, 3, 0), Offset.FromHours(3)), zdt.ToOffsetDateTime());
            zdt = zdt.Plus(Duration.FromStandardDays(180));
            Assert.AreEqual(new OffsetDateTime(new LocalDateTime(2008, 6, 29, 4, 0), Offset.FromHours(4)), zdt.ToOffsetDateTime());
        }

        [Test]
        public void ToInstant()
        {
            Assert.AreEqual(new Instant(14313418200000000), new ZonedDateTime(Instant.FromUtc(2015, 5, 11, 10, 57), DateTimeZone.ForOffset(Offset.FromHours(3))).ToInstant());
            Assert.AreEqual(new Instant(16200000000), new ZonedDateTime(Instant.FromUtc(1970, 1, 1, 0, 27), DateTimeZone.ForOffset(Offset.FromHours(2))).ToInstant());
        }

        [Test]
        public void Equality()
        {
            var odt1 = new ZonedDateTime(Instant.FromUtc(2015, 5, 11, 10, 57), DateTimeZone.ForOffset(Offset.FromHours(3)));
            var odt2 = new ZonedDateTime(Instant.FromUtc(2015, 5, 11, 10, 57), DateTimeZone.ForOffset(Offset.FromHours(4)));
            var odt3 = new ZonedDateTime(Instant.FromUtc(2015, 5, 11, 10, 57), DateTimeZone.ForOffset(Offset.FromHours(3)));
            var odt4 = new ZonedDateTime(Instant.FromUtc(2015, 5, 12, 10, 57), DateTimeZone.ForOffset(Offset.FromHours(4)));

            Assert.IsTrue(odt1 == odt3);
            Assert.IsTrue(odt1 != odt2);
            Assert.IsTrue(odt1 != odt4);
            Assert.IsTrue(odt3 != odt4);
        }

        [Test]
        public void SimpleProperties()
        {
            var value = new ZonedDateTime(new LocalDateTime(2012, 2, 10, 8, 9, 10, 11, 12), DateTimeZone.ForOffset(Offset.Zero));
            Assert.AreEqual(new LocalDate(2012, 2, 10), value.Date);
            Assert.AreEqual(new LocalTime(8, 9, 10, 11, 12), value.TimeOfDay);
            Assert.AreEqual(Era.Common, value.Era);
            Assert.AreEqual(20, value.CenturyOfEra);
            Assert.AreEqual(12, value.YearOfCentury);
            Assert.AreEqual(2012, value.Year);
            Assert.AreEqual(2012, value.YearOfEra);
            Assert.AreEqual(2, value.Month);
            Assert.AreEqual(10, value.Day);
            Assert.AreEqual(6, value.WeekOfWeekYear);
            Assert.AreEqual(2012, value.WeekYear);
            Assert.AreEqual(IsoDayOfWeek.Friday, value.IsoDayOfWeek);
            Assert.AreEqual((int) IsoDayOfWeek.Friday, value.DayOfWeek);
            Assert.AreEqual(41, value.DayOfYear);
            Assert.AreEqual(8, value.ClockHourOfHalfDay);
            Assert.AreEqual(8, value.Hour);
            Assert.AreEqual(9, value.Minute);
            Assert.AreEqual(10, value.Second);
            Assert.AreEqual(11, value.Millisecond);
            Assert.AreEqual(11 * 10000 + 12, value.TickOfSecond);
            Assert.AreEqual(8 * Constants.TicksPerHour +
                            9 * Constants.TicksPerMinute +
                            10 * Constants.TicksPerSecond +
                            11 * Constants.TicksPerMillisecond +
                            12,
                            value.TickOfDay);
        }
    }
}
