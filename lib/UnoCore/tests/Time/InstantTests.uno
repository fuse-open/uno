using Uno;
using Uno.Text;
using Uno.Testing;
using Uno.Time;

namespace Uno.Time.Test
{
    public class InstantTests
    {
        [Test]
        public void DefaultConstructor()
        {
            var actual = new Instant();
            Assert.AreEqual(0, actual.Ticks);
        }

        [Test]
        public void FromUtc()
        {
            Assert.IsFalse(CalendarSystem.Iso == null);
            Assert.AreEqual(14231018400000000, Instant.FromUtc(2015, 2, 5, 2, 4).Ticks);
        }

        [Test]
        public void Plus()
        {
            Assert.AreEqual(14243114400000000, Instant.FromUtc(2015, 2, 5, 2, 4).Plus(Duration.FromStandardWeeks(2)).Ticks);
        }

        [Test]
        public void Substract()
        {
            Assert.AreEqual(14218922400000000, (Instant.FromUtc(2015, 2, 5, 2, 4) - Duration.FromStandardWeeks(2)).Ticks);
        }

        [Test]
        public void Operators()
        {
            Instant x = new Instant(100);
            Instant y = new Instant(200);
            Assert.IsTrue(x == new Instant(100));
            Assert.IsTrue(x != y);
            Assert.IsTrue(x < y);
            Assert.IsTrue(x <=y);
            Assert.IsTrue(y >= x);
            Assert.IsTrue(y > x);
        }
    }
}
