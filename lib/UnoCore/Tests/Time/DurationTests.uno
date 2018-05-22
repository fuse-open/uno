using Uno;
using Uno.Text;
using Uno.Testing;
using Uno.Time;

namespace Uno.Time.Test
{
    public class DurationTests
    {
        [Test]
        public void DefaultConstructor()
        {
            var actual = new Duration();
            Assert.AreEqual(Duration.Zero, actual);
        }

        [Test]
        public void Operators()
        {
            Duration x = Duration.FromMinutes(100);
            Duration y = Duration.FromMinutes(200);
            Assert.IsTrue(x == Duration.FromMinutes(100));
            Assert.IsTrue(x != y);
            Assert.IsTrue(x < y);
            Assert.IsTrue(x <=y);
            Assert.IsTrue(y >= x);
            Assert.IsTrue(y > x);
        }

        [Test]
        public void Operations()
        {
            Duration x = Duration.FromMinutes(10);
            Assert.AreEqual(-6000000000, (-x).Ticks);
            Duration y = Duration.FromMinutes(20);
            Assert.AreEqual(18000000000, (x + y).Ticks);
            Assert.AreEqual(-6000000000, (x - y).Ticks);
            Assert.AreEqual(6000000000, (y - x).Ticks);
            Assert.AreEqual(12000000000, x.Plus(x).Ticks);
            Assert.AreEqual(90000000000, (x * 15).Ticks);
            Assert.AreEqual(1000000000, (x / 6).Ticks);
        }

        /*[Test]
        public void ToString()
        {
            Assert.AreEqual("15:00:00:00", (Duration.FromStandardWeeks(2) + Duration.FromStandardDays(1)).ToString());
            Assert.AreEqual("-13:04:00:00", (Duration.FromStandardWeeks(-2) + Duration.FromHours(20)).ToString());
        }*/
    }
}
