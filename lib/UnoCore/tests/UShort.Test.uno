using Uno;
using Uno.Testing;

namespace Uno.Test
{
    public class UShortTest
    {
        [Test]
        public void TryParse()
        {
            ushort res;
            Assert.IsTrue(ushort.TryParse(" +14 ", out res));
            Assert.AreEqual(14, res);
            Assert.IsFalse(ushort.TryParse("str", out res));
            Assert.AreEqual(0, res);
            Assert.IsFalse(ushort.TryParse(" 654684684 ", out res));
            Assert.AreEqual(0, res);
            Assert.IsFalse(ushort.TryParse("-5", out res));
            Assert.AreEqual(0, res);
        }

        [Test]
        public void ParseValidValue()
        {
            Assert.AreEqual(9, ushort.Parse("09"));
            Assert.AreEqual(250, ushort.Parse("250"));
            Assert.AreEqual(5, ushort.Parse("+5"));
            Assert.AreEqual(19, ushort.Parse(" 19"));
            Assert.AreEqual(21, ushort.Parse("21 "));
            Assert.AreEqual(100, ushort.Parse(" 100  "));
            Assert.AreEqual(ushort.MaxValue, ushort.Parse(ushort.MaxValue.ToString()));
            Assert.AreEqual(ushort.MinValue, ushort.Parse(ushort.MinValue.ToString()));
        }

        [Test]
        public void ParseInvalidValue()
        {
            Assert.Throws<OverflowException>(ParseHugeValueAction);
            Assert.Throws<OverflowException>(ParseNegativeValueAction);
            Assert.Throws<FormatException>(ParseStringAction);
            Assert.Throws<FormatException>(ParseStringAction2);
            Assert.Throws<FormatException>(ParseEmptyStringAction);
            Assert.Throws<ArgumentNullException>(ParseNullAction);
        }

        void ParseHugeValueAction()
        {
            ushort.Parse((ushort.MaxValue * 2).ToString());
        }

        void ParseNegativeValueAction()
        {
            ushort.Parse("-10");
        }

        void ParseStringAction()
        {
            ushort.Parse("just_string");
        }

        void ParseStringAction2()
        {
            ushort.Parse("11foo");
        }

        void ParseEmptyStringAction()
        {
            ushort.Parse("");
        }

        void ParseNullAction()
        {
            ushort.Parse(null);
        }
    }
}
