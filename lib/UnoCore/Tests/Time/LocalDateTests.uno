using Uno;
using Uno.Text;
using Uno.Testing;
using Uno.Time;

namespace Uno.Time.Test
{
    public class LocalDateTests
    {
        [Test]
        public void CombinationWithTime()
        {
            LocalDate date = new LocalDate(2014, 3, 28);
            LocalTime time = new LocalTime(20, 17, 30);
            LocalDateTime expected = new LocalDateTime(2014, 3, 28, 20, 17, 30);
            Assert.AreEqual(expected, date + time);
            Assert.AreEqual(expected, date.At(time));
            Assert.AreEqual(expected, time.On(date));
        }

        [Test]
        public void Compare()
        {
            var ld1 = new LocalDate(2011, 12, 5);
            var ld2 = new LocalDate(2011, 12, 6);
            var ld3 = new LocalDate(2010, 12, 5);

            Assert.IsTrue(ld1 >= ld1);
            Assert.IsFalse(ld1 < ld1);
            Assert.IsTrue(ld1 <= ld2);
            Assert.IsFalse(ld1 < ld3);
        }

        [Test]
        public void ToString()
        {
            Assert.AreEqual("2012-05-29", new LocalDate(2012, 5, 29).ToString());
            Assert.AreEqual("0907-12-09", new LocalDate(907, 12, 9).ToString());
            Assert.AreEqual("-0012-05-03", new LocalDate(-12, 5, 3).ToString());
        }
    }
}
