using Uno;
using Uno.Text;
using Uno.Testing;
using Uno.Time;

namespace Uno.Time.Test
{
    public class OffsetTests
    {
        [Test]
        public void Max()
        {
            Offset x = Offset.FromMilliseconds(100);
            Offset y = Offset.FromMilliseconds(200);
            Assert.AreEqual(y, Offset.Max(x, y));
            Assert.AreEqual(y, Offset.Max(y, x));
            Assert.AreEqual(x, Offset.Max(x, Offset.MinValue));
            Assert.AreEqual(x, Offset.Max(Offset.MinValue, x));
            Assert.AreEqual(Offset.MaxValue, Offset.Max(Offset.MaxValue, x));
            Assert.AreEqual(Offset.MaxValue, Offset.Max(x, Offset.MaxValue));
        }

        [Test]
        public void Min()
        {
            Offset x = Offset.FromMilliseconds(100);
            Offset y = Offset.FromMilliseconds(200);
            Assert.AreEqual(x, Offset.Min(x, y));
            Assert.AreEqual(x, Offset.Min(y, x));
            Assert.AreEqual(Offset.MinValue, Offset.Min(x, Offset.MinValue));
            Assert.AreEqual(Offset.MinValue, Offset.Min(Offset.MinValue, x));
            Assert.AreEqual(x, Offset.Min(Offset.MaxValue, x));
            Assert.AreEqual(x, Offset.Min(x, Offset.MaxValue));
        }

        [Test]
        public void DefaultConstructor()
        {
            var actual = new Offset();
            Assert.AreEqual(Offset.Zero, actual);
            Assert.AreEqual(3600000, Offset.FromHours(1).Milliseconds);
        }

        [Test]
        public void Operators()
        {
            Offset x = Offset.FromMilliseconds(100);
            Offset y = Offset.FromMilliseconds(200);
            Assert.IsTrue(x == Offset.FromMilliseconds(100));
            Assert.IsTrue(x != y);
            Assert.IsTrue(x < y);
            Assert.IsTrue(x <=y);
            Assert.IsTrue(y >= x);
            Assert.IsTrue(y > x);
        }

        [Test]
        public void Operations()
        {
            Offset x = Offset.FromMilliseconds(100);
            Assert.AreEqual(-100, (-x).Milliseconds);
            Offset y = Offset.FromMilliseconds(200);
            Assert.AreEqual(300, (x + y).Milliseconds);
            Assert.AreEqual(-100, (x - y).Milliseconds);
            Assert.AreEqual(100, (y - x).Milliseconds);
            Assert.AreEqual(200, x.Plus(x).Milliseconds);
        }
    }
}
