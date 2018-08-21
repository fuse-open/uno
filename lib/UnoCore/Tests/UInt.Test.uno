using Uno;
using Uno.Testing;

namespace Uno.Test
{
    public class UIntTest
    {
        [Test]
        public void TryParse()
        {
            uint res;
            Assert.IsTrue(uint.TryParse(" +14 ", out res));
            Assert.AreEqual(14, res);
            Assert.IsFalse(uint.TryParse("649849846465432135149684984684", out res));
            Assert.AreEqual(0, res);
            Assert.IsFalse(uint.TryParse("str", out res));
            Assert.AreEqual(0, res);
            Assert.IsFalse(uint.TryParse("-5", out res));
            Assert.AreEqual(0, res);
        }

        [Test]
        public void ParseValidValue()
        {
            Assert.AreEqual(9, uint.Parse("09"));
            Assert.AreEqual(55, uint.Parse("55"));
            Assert.AreEqual(5, uint.Parse("+5"));
            Assert.AreEqual(19, uint.Parse(" 19"));
            Assert.AreEqual(21, uint.Parse("21 "));
            Assert.AreEqual(100, uint.Parse(" 100  "));
            Assert.AreEqual(uint.MaxValue, uint.Parse(uint.MaxValue.ToString()));
            Assert.AreEqual(uint.MinValue, uint.Parse(uint.MinValue.ToString()));
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
            uint.Parse((uint.MaxValue * 2L).ToString());
        }

        void ParseNegativeValueAction()
        {
            uint.Parse("-10");
        }

        void ParseStringAction()
        {
            uint.Parse("just_string");
        }

        void ParseStringAction2()
        {
            uint.Parse("11foo");
        }

        void ParseEmptyStringAction()
        {
            uint.Parse("");
        }

        void ParseNullAction()
        {
            uint.Parse(null);
        }
    }
}
