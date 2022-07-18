using Uno;
using Uno.Text;
using Uno.Testing;

namespace Uno.Time.Test
{
    public class LocalTimeTests
    {
        [Test]
        public void ClockHourOfHalfDay()
        {
            Assert.AreEqual(12, new LocalTime(0, 0).ClockHourOfHalfDay);
            Assert.AreEqual(1, new LocalTime(1, 0).ClockHourOfHalfDay);
            Assert.AreEqual(12, new LocalTime(12, 0).ClockHourOfHalfDay);
            Assert.AreEqual(1, new LocalTime(13, 0).ClockHourOfHalfDay);
            Assert.AreEqual(11, new LocalTime(23, 0).ClockHourOfHalfDay);
        }

        [Test]
        public void DefaultConstructor()
        {
            var actual = new LocalTime();
            Assert.AreEqual(LocalTime.Midnight, actual);
        }

        [Test]
        public void Compare()
        {
            var lt1 = new LocalTime(15, 6, 59);
            var lt2 = new LocalTime(15, 7, 0);
            var lt3 = new LocalTime(12, 6, 59);

            Assert.IsTrue(lt1 >= lt1);
            Assert.IsFalse(lt1 < lt1);
            Assert.IsTrue(lt1 <= lt2);
            Assert.IsFalse(lt1 < lt3);
        }

        [Test]
        public void ToString()
        {
            Assert.AreEqual("02:00:59", new LocalTime(2, 0, 59).ToString());
            Assert.AreEqual("23:12:09", new LocalTime(23, 12, 9).ToString());
        }
    }
}
