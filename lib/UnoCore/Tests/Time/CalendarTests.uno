using Uno;
using Uno.Text;
using Uno.Testing;
using Uno.Time;
using Uno.Time.Calendars;
using System.IO;

namespace Uno.Time.Test
{
    public class CalendarTests
    {
        [Test]
        public void FieldsOf_UnixEpoch()
        {
            LocalDateTime epoch = new LocalDateTime(new Instant(0), CalendarSystem.Iso);

            Assert.AreEqual(1970, epoch.Year);
            Assert.AreEqual(1970, epoch.YearOfEra);
            Assert.AreEqual(70, epoch.YearOfCentury);
            Assert.AreEqual(19, epoch.CenturyOfEra);
            Assert.AreEqual(1970, epoch.WeekYear);

            Assert.AreEqual(1, epoch.WeekOfWeekYear);
            Assert.AreEqual(1, epoch.Month);
            Assert.AreEqual(1, epoch.Day);
            Assert.AreEqual(1, epoch.DayOfYear);
            Assert.AreEqual(IsoDayOfWeek.Thursday, epoch.IsoDayOfWeek);
            Assert.AreEqual(4, epoch.DayOfWeek);

            Assert.AreEqual(Era.Common, epoch.Era);

            Assert.AreEqual(0, epoch.Hour);
            Assert.AreEqual(0, epoch.Minute);
            Assert.AreEqual(0, epoch.Second);
            Assert.AreEqual(0, epoch.Millisecond);
            Assert.AreEqual(0, epoch.TickOfDay);
            Assert.AreEqual(0, epoch.TickOfSecond);
        }


    }
}
