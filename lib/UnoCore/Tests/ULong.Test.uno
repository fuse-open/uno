using Uno;
using Uno.Testing;

namespace Uno.Test
{
    public class ULongTest
    {
        [Test]
        public void TryParse()
        {
            ulong res;
            Assert.IsTrue(ulong.TryParse(" 48545 ", out res));
            Assert.AreEqual(48545, res);
            Assert.IsFalse(ulong.TryParse(" -15 ", out res));
            Assert.AreEqual(0, res);
            Assert.IsFalse(ulong.TryParse("str", out res));
            Assert.AreEqual(0, res);
            Assert.IsFalse(ulong.TryParse("0.12", out res));
            Assert.AreEqual(0, res);
        }

        [Test]
        public void ParseValidValue()
        {
            Assert.AreEqual(9, ulong.Parse("09"));
            Assert.AreEqual(55555, ulong.Parse("55555"));
            Assert.AreEqual(55, ulong.Parse("+55"));
            Assert.AreEqual(19, ulong.Parse(" 19"));
            Assert.AreEqual(21, ulong.Parse("21 "));
            Assert.AreEqual(100, ulong.Parse(" 100  "));
            Assert.AreEqual(ulong.MaxValue, ulong.Parse(ulong.MaxValue.ToString()));
            Assert.AreEqual(ulong.MinValue, ulong.Parse(ulong.MinValue.ToString()));
        }

        [Test]
        public void ParseInvalidValue()
        {
            Assert.Throws<OverflowException>(ParseNegativeValueAction);
            Assert.Throws<FormatException>(ParseStringAction);
            Assert.Throws<FormatException>(ParseStringAction2);
            Assert.Throws<FormatException>(ParseEmptyStringAction);
            Assert.Throws<ArgumentNullException>(ParseNullAction);
        }

        void ParseNegativeValueAction()
        {
            ulong.Parse("-10");
        }

        void ParseStringAction()
        {
            ulong.Parse("just_string");
        }

        void ParseStringAction2()
        {
            ulong.Parse("11foo");
        }

        void ParseEmptyStringAction()
        {
            ulong.Parse("");
        }

        void ParseNullAction()
        {
            ulong.Parse(null);
        }
    }
}
