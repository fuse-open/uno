using Uno;
using Uno.Text;
using Uno.Testing;
using Uno.Time;

namespace Uno.Time.Test
{
    public class OffsetDateTimeTests
    {
        [Test]
        public void OffsetProperty()
        {
            Offset offset = Offset.FromHours(5);

            OffsetDateTime odt = new OffsetDateTime(new LocalDateTime(2012, 1, 2, 3, 4), offset);
            Assert.AreEqual(offset, odt.Offset);
        }

        [Test]
        public void ToInstant()
        {
            Assert.AreEqual(new Instant(14314370400000000), new OffsetDateTime(new LocalDateTime(2015, 5, 12, 13, 24), Offset.Zero).ToInstant());
            Assert.AreEqual(new Instant(14314370390000000), new OffsetDateTime(new LocalDateTime(2015, 5, 12, 13, 24), Offset.FromTicks(10000000)).ToInstant());
        }

        [Test]
        public void Equality()
        {
            var odt1 = new OffsetDateTime(new LocalDateTime(2015, 5, 12, 13, 24), Offset.Zero);
            var odt2 = new OffsetDateTime(new LocalDateTime(2015, 5, 12, 10, 24), Offset.FromHours(3));
            var odt3 = new OffsetDateTime(new LocalDateTime(2015, 5, 12, 13, 24, 0), Offset.Zero);
            var odt4 = new OffsetDateTime(new LocalDateTime(2015, 5, 12, 13, 25), Offset.Zero);

            Assert.IsTrue(odt1 == odt3);
            Assert.IsTrue(odt1 != odt2);
            Assert.IsTrue(odt1 != odt4);
            Assert.IsTrue(odt3 != odt4);
        }

        [Test]
        public void Compare()
        {
            var odt1 = new OffsetDateTime(new LocalDateTime(2015, 5, 12, 13, 24), Offset.Zero);
            var odt2 = new OffsetDateTime(new LocalDateTime(2015, 5, 12, 10, 24), Offset.FromHours(3));
            var odt3 = new OffsetDateTime(new LocalDateTime(2015, 5, 12, 13, 24, 0), Offset.Zero);
            var odt4 = new OffsetDateTime(new LocalDateTime(2015, 5, 12, 13, 25), Offset.Zero);

            Assert.IsTrue(odt1 == odt3);
            Assert.IsTrue(odt1 != odt2);
            Assert.IsTrue(odt1 != odt4);
            Assert.IsTrue(odt3 != odt4);
        }

        [Test]
        public void ToString()
        {
            Assert.AreEqual("2323-02-24T02:00:59+01:00:00", new OffsetDateTime(new LocalDateTime(2323, 2, 24, 2, 0, 59), Offset.FromHours(1)).ToString());
            Assert.AreEqual("-0242-11-02T00:00:00-11:33:00", new OffsetDateTime(new LocalDateTime(-242, 11, 02, 0, 0, 0), Offset.FromHoursAndMinutes(-11, -33)).ToString());
        }
    }
}
