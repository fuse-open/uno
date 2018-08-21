using Uno;
using Uno.Testing;

namespace Uno.Test
{
    public class IntTest
    {
        [Test]
        public void TryParse()
        {
            int res;
            Assert.IsTrue(int.TryParse("554", out res));
            Assert.AreEqual(554, res);
            Assert.IsTrue(int.TryParse(" +0554 ", out res));
            Assert.AreEqual(554, res);
            Assert.IsFalse(int.TryParse("4654984984984984984984", out res));
            Assert.AreEqual(0, res);
            Assert.IsFalse(int.TryParse("str", out res));
            Assert.AreEqual(0, res);
            Assert.IsFalse(int.TryParse("0.5", out res));
            Assert.AreEqual(0, res);
        }

        [Test]
        public void ParseValidValue()
        {
            Assert.AreEqual(9, int.Parse("09"));
            Assert.AreEqual(55, int.Parse("55"));
            Assert.AreEqual(70, int.Parse("+70"));
            Assert.AreEqual(-55, int.Parse("-55"));
            Assert.AreEqual(50, int.Parse(" 50"));
            Assert.AreEqual(30, int.Parse("30 "));
            Assert.AreEqual(100, int.Parse(" 100 "));
            Assert.AreEqual(0, int.Parse("0"));
            Assert.AreEqual(int.MaxValue, int.Parse(int.MaxValue.ToString()));
            Assert.AreEqual(int.MinValue, int.Parse(int.MinValue.ToString()));
        }

        [Test]
        public void ParseInvalidValue()
        {
            Assert.Throws<OverflowException>(ParseHugeValueAction);
            Assert.Throws<OverflowException>(ParseHugeNegativeValueAction);
            Assert.Throws<FormatException>(ParseStringAction);
            Assert.Throws<FormatException>(ParseStringAction2);
            Assert.Throws<FormatException>(ParseEmptyStringAction);
            Assert.Throws<FormatException>(ParseFloatAction);
            Assert.Throws<ArgumentNullException>(ParseNullAction);
        }

        void ParseHugeValueAction()
        {
            int.Parse((int.MaxValue * 2L).ToString());
        }

        void ParseHugeNegativeValueAction()
        {
            int.Parse((int.MinValue * 2L).ToString());
        }

        void ParseStringAction()
        {
            int.Parse("just_string");
        }

        void ParseStringAction2()
        {
            int.Parse("50foo");
        }

        void ParseFloatAction()
        {
            int.Parse("50.5");
        }

        void ParseEmptyStringAction()
        {
            int.Parse("");
        }

        void ParseNullAction()
        {
            int.Parse(null);
        }
    }
}
