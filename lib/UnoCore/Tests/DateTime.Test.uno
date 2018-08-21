using Uno;
using Uno.Testing;

namespace Uno.Test
{
    public class DateTimeTest
    {
        // Unix epoch is January 1, 1970 at 00:00:00.000
        const long unixEpochTicks = 621355968000000000L;

        [Test]
        public void ZeroCtor()
        {
            // Zero ticks should represent January 1, 0001 at 00:00:00.000
            var dt = new DateTime(0L, DateTimeKind.Utc);

            Assert.AreEqual(DateTimeKind.Utc, dt.Kind);

            Assert.AreEqual(0, dt.Ticks);

            Assert.AreEqual(1, dt.Year);
            Assert.AreEqual(1, dt.Month);
            Assert.AreEqual(1, dt.Day);
            Assert.AreEqual(0, dt.Hour);
            Assert.AreEqual(0, dt.Minute);
            Assert.AreEqual(0, dt.Second);
        }

        [Test]
        public void UnixEpochCtor()
        {
            var dt = new DateTime(unixEpochTicks, DateTimeKind.Utc);

            Assert.AreEqual(DateTimeKind.Utc, dt.Kind);

            Assert.AreEqual(unixEpochTicks, dt.Ticks);

            Assert.AreEqual(1970, dt.Year);
            Assert.AreEqual(1, dt.Month);
            Assert.AreEqual(1, dt.Day);
            Assert.AreEqual(0, dt.Hour);
            Assert.AreEqual(0, dt.Minute);
            Assert.AreEqual(0, dt.Second);
        }

        [Test]
        public void SpecificDateCtor()
        {
            // Represents June 23, 1996 at 13:37:32.0000 (Nintendo 64 release date (JP) plus some arbitrary hours/minutes/seconds)
            const long ticks = 629711338520000000L;

            var dt = new DateTime(ticks, DateTimeKind.Utc);

            Assert.AreEqual(DateTimeKind.Utc, dt.Kind);

            Assert.AreEqual(ticks, dt.Ticks);

            Assert.AreEqual(1996, dt.Year);
            Assert.AreEqual(6, dt.Month);
            Assert.AreEqual(23, dt.Day);
            Assert.AreEqual(13, dt.Hour);
            Assert.AreEqual(37, dt.Minute);
            Assert.AreEqual(32, dt.Second);
        }

        [Test]
        public void UtcNowEqualsSelf()
        {
            var dt = DateTime.UtcNow;

            Assert.AreEqual(dt, dt);
            Assert.IsTrue(dt == dt);
            Assert.IsFalse(dt != dt);
            Assert.IsTrue(dt.Equals(dt));
        }

        [Test]
        public void UtcNowIsNotNull()
        {
            var dt = DateTime.UtcNow;

            Assert.AreNotEqual(dt, null);
            Assert.AreNotEqual(null, dt);
            Assert.IsFalse(dt.Equals(null));
        }

        [Test]
        public void UtcNowIsNotSomethingElse()
        {
            var dt = DateTime.UtcNow;
            var somethingElse = float2(13.0f, 37.0f);

            Assert.AreNotEqual(dt, somethingElse);
            Assert.IsFalse(dt.Equals(somethingElse));
        }

        [Test]
        public void TwoInstancesWithSameTicksAreEqual()
        {
            var x = new DateTime(0, DateTimeKind.Utc);
            var y = new DateTime(0, DateTimeKind.Utc);

            Assert.AreEqual(x, y);
            Assert.IsTrue(x == y);
            Assert.IsFalse(x != y);
            Assert.IsTrue(x.Equals(y));
            Assert.IsTrue(y.Equals(x));
        }

        [Test]
        public void TwoInstancesWithDifferentTicksAreNotEqual()
        {
            var x = new DateTime(0, DateTimeKind.Utc);
            var y = new DateTime(1, DateTimeKind.Utc);

            Assert.AreNotEqual(x, y);
            Assert.IsTrue(x != y);
            Assert.IsFalse(x == y);
            Assert.IsFalse(x.Equals(y));
            Assert.IsFalse(y.Equals(x));
        }

        [Test]
        public void DefaultCtorIsDotNetBase()
        {
            var dt = new DateTime();

            Assert.AreEqual(dt.Ticks, 0);
            Assert.AreEqual(dt.Kind, (DateTimeKind)0);
        }

        [Test]
        public void ToUniversalTimeWithKindUtcReturnsSameValue()
        {
            var x = new DateTime(0, DateTimeKind.Utc);
            var xUtc = x.ToUniversalTime();

            Assert.AreEqual(0, xUtc.Ticks);
            Assert.AreEqual(DateTimeKind.Utc, xUtc.Kind);
        }

        [Test]
        public void ToString()
        {
            var a = new DateTime(0, DateTimeKind.Utc);
            var b = new DateTime(unixEpochTicks, DateTimeKind.Utc);
            var c = new DateTime(111333333777L, DateTimeKind.Utc);
            var d = new DateTime(91827364546372642L, DateTimeKind.Utc);

            Assert.AreEqual("01/01/0001 00:00:00", a.ToString());
            Assert.AreEqual("01/01/1970 00:00:00", b.ToString());
            Assert.AreEqual("01/01/0001 03:05:33", c.ToString());
            Assert.AreEqual("12/28/0291 16:07:34", d.ToString());
        }
    }
}
