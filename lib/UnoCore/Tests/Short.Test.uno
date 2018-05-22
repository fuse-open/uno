using Uno;
using Uno.Testing;

namespace Uno.Test
{
    public class ShortTest
    {
        [Test]
        public void TryParse()
        {
            short res;
            Assert.IsTrue(short.TryParse(" +32000 ", out res));
            Assert.AreEqual(32000, res);
            Assert.IsFalse(short.TryParse("5646546546", out res));
            Assert.AreEqual(0, res);
            Assert.IsFalse(short.TryParse("str", out res));
            Assert.AreEqual(0, res);
            Assert.IsFalse(short.TryParse("0.5", out res));
            Assert.AreEqual(0, res);
        }

        [Test]
        public void ParseValidValue()
        {
            Assert.AreEqual(9, short.Parse("09"));
            Assert.AreEqual(0, short.Parse("0"));
            Assert.AreEqual(33, short.Parse("33"));
            Assert.AreEqual(-22, short.Parse("-22"));
            Assert.AreEqual(5, short.Parse("+5"));
            Assert.AreEqual(19, short.Parse(" 19"));
            Assert.AreEqual(-21, short.Parse("-21 "));
            Assert.AreEqual(100, short.Parse(" 100  "));
            Assert.AreEqual(short.MaxValue, short.Parse(short.MaxValue.ToString()));
            Assert.AreEqual(short.MinValue, short.Parse(short.MinValue.ToString()));
        }

        [Test]
        public void ParseInvalidValue()
        {
            Assert.Throws<OverflowException>(ParseHugeValueAction);
            Assert.Throws<OverflowException>(ParseHugeNegativeValueAction);
            Assert.Throws<FormatException>(ParseStringAction);
            Assert.Throws<FormatException>(ParseStringAction2);
            Assert.Throws<FormatException>(ParseEmptyStringAction);
            Assert.Throws<ArgumentNullException>(ParseNullAction);
        }

        void ParseHugeValueAction()
        {
            short.Parse((short.MaxValue * 2).ToString());
        }

        void ParseHugeNegativeValueAction()
        {
            short.Parse((short.MinValue * 2).ToString());
        }

        void ParseStringAction()
        {
            short.Parse("just_string");
        }

        void ParseStringAction2()
        {
            short.Parse("11foo");
        }

        void ParseEmptyStringAction()
        {
            short.Parse("");
        }

        void ParseNullAction()
        {
            short.Parse(null);
        }
    }
}
