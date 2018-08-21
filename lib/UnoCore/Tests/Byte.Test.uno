using Uno;
using Uno.Testing;

namespace Uno.Test
{
    public class ByteTest
    {
        [Test]
        public void TryParse()
        {
            byte res;
            Assert.IsTrue(byte.TryParse(" +051 ", out res));
            Assert.AreEqual(51, res);
            Assert.IsFalse(byte.TryParse("str", out res));
            Assert.AreEqual(0, res);
            Assert.IsFalse(byte.TryParse("51561654", out res));
            Assert.AreEqual(0, res);
            Assert.IsFalse(byte.TryParse("0.5", out res));
            Assert.AreEqual(0, res);
        }

        [Test]
        public void ParseValidValue()
        {
            Assert.AreEqual(9, byte.Parse("09"));
            Assert.AreEqual(3, byte.Parse("3"));
            Assert.AreEqual(5, byte.Parse("+5"));
            Assert.AreEqual(19, byte.Parse(" 19"));
            Assert.AreEqual(21, byte.Parse("21 "));
            Assert.AreEqual(100, byte.Parse(" 100  "));
            Assert.AreEqual(byte.MaxValue, byte.Parse(byte.MaxValue.ToString()));
            Assert.AreEqual(byte.MinValue, byte.Parse(byte.MinValue.ToString()));
        }

        [Test]
        public void ParseInvalidValue()
        {
            Assert.Throws<OverflowException>(ParseNegativeValueAction);
            Assert.Throws<OverflowException>(ParseHugeValueAction);
            Assert.Throws<FormatException>(ParseStringAction);
            Assert.Throws<FormatException>(ParseStringAction2);
            Assert.Throws<FormatException>(ParseEmptyStringAction);
            Assert.Throws<ArgumentNullException>(ParseNullAction);
        }

        void ParseHugeValueAction()
        {
            byte.Parse((byte.MaxValue * 2).ToString());
        }

        void ParseNegativeValueAction()
        {
            byte.Parse("-10");
        }

        void ParseStringAction()
        {
            byte.Parse("just_string");
        }

        void ParseStringAction2()
        {
            byte.Parse("50foo");
        }

        void ParseEmptyStringAction()
        {
            byte.Parse("");
        }

        void ParseNullAction()
        {
            byte.Parse(null);
        }
    }
}
