using Uno;
using Uno.Testing;

namespace Uno.Test
{
    public class LongTest
    {
        [Test]
        public void TryParse()
        {
            long res;
            Assert.IsTrue(long.TryParse(" -554 ", out res));
            Assert.AreEqual(-554, res);
            Assert.IsFalse(long.TryParse(" 849849849849849849849849654654554 ", out res));
            Assert.AreEqual(0, res);
            Assert.IsFalse(long.TryParse("str", out res));
            Assert.AreEqual(0, res);
            Assert.IsFalse(long.TryParse("0.5", out res));
            Assert.AreEqual(0, res);
        }

        [Test]
        public void ParseValidValue()
        {
            Assert.AreEqual(9, long.Parse("09"));
            Assert.AreEqual(32, long.Parse("32"));
            Assert.AreEqual(-55, long.Parse("-55"));
            Assert.AreEqual(5, long.Parse("+5"));
            Assert.AreEqual(19, long.Parse(" 19"));
            Assert.AreEqual(21, long.Parse("21 "));
            Assert.AreEqual(100, long.Parse(" 100  "));
            Assert.AreEqual(0, long.Parse("0"));
            Assert.AreEqual(long.MaxValue, long.Parse(long.MaxValue.ToString()));
            Assert.AreEqual(long.MinValue, long.Parse(long.MinValue.ToString()));
        }

        [Test]
        public void ParseInvalidValue()
        {
            Assert.Throws<FormatException>(ParseStringAction);
            Assert.Throws<FormatException>(ParseStringAction2);
            Assert.Throws<FormatException>(ParseEmptyStringAction);
            Assert.Throws<FormatException>(ParseFloatAction);
            Assert.Throws<ArgumentNullException>(ParseNullAction);
        }

        void ParseStringAction()
        {
            long.Parse("just_string");
        }

        void ParseStringAction2()
        {
            long.Parse("5000foo");
        }

        void ParseEmptyStringAction()
        {
            long.Parse("");
        }

        void ParseFloatAction()
        {
            long.Parse("50.5");
        }

        void ParseNullAction()
        {
            long.Parse(null);
        }
    }
}
