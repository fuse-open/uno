using Uno;
using Uno.Testing;

namespace Uno.Test
{
    public class FloatTest
    {
        [Test]
        public void IsNaN()
        {
            Assert.IsTrue(float.IsNaN(float.NaN));
            Assert.IsTrue(float.IsNaN(.0f/.0f));
            Assert.IsFalse(float.IsNaN(1.0f/.0f));
            Assert.IsFalse(float.IsNaN(-1.0f/.0f));
            Assert.IsFalse(float.IsNaN(1.1f));
        }

        [Test]
        public void IsInfinity()
        {
            Assert.IsTrue(float.IsInfinity(float.PositiveInfinity));
            Assert.IsTrue(float.IsInfinity(float.NegativeInfinity));
            Assert.IsFalse(float.IsInfinity(float.NaN));
            Assert.IsFalse(float.IsInfinity(1.1f));
        }

        [Test]
        public void IsPositiveInfinity()
        {
            Assert.IsTrue(float.IsPositiveInfinity(float.PositiveInfinity));
            Assert.IsFalse(float.IsPositiveInfinity(float.NegativeInfinity));
            Assert.IsFalse(float.IsPositiveInfinity(float.NaN));
            Assert.IsFalse(float.IsPositiveInfinity(1.1f));
        }

        [Test]
        public void IsNegativeInfinity()
        {
            Assert.IsTrue(float.IsNegativeInfinity(float.NegativeInfinity));
            Assert.IsFalse(float.IsNegativeInfinity(float.PositiveInfinity));
            Assert.IsFalse(float.IsNegativeInfinity(float.NaN));
            Assert.IsFalse(float.IsNegativeInfinity(1.1f));
        }

        [Test]
        public void TryParse()
        {
            float res;
            Assert.IsTrue(float.TryParse(" +554.2 ", out res));
            Assert.AreEqual(554.2f, res);
            Assert.IsFalse(float.TryParse("849849849846546546548979876432213568491", out res));
            Assert.AreEqual(0, res);
            Assert.IsFalse(float.TryParse("str", out res));
            Assert.AreEqual(0, res);
        }

        [Test]
        public void ParseValidValue()
        {
            Assert.AreEqual(32.5f, float.Parse("32.5"));
            Assert.AreEqual(-55.2f, float.Parse("-55.2"));
            Assert.AreEqual(2.6f, float.Parse("+2.6"));
            Assert.AreEqual(-5.2f, float.Parse("-5.2 "));
            Assert.AreEqual(85.2f, float.Parse(" 85.2"));
            Assert.AreEqual(100, float.Parse(" 100 "));
            Assert.AreEqual(0, float.Parse("0"));
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
            float.Parse((float.MaxValue * 2.0).ToString());
        }

        void ParseHugeNegativeValueAction()
        {
            float.Parse((float.MinValue * 2.0).ToString());
        }

        void ParseStringAction()
        {
            float.Parse("just_string");
        }

        void ParseStringAction2()
        {
            float.Parse("50.0foo");
        }

        void ParseEmptyStringAction()
        {
            float.Parse("");
        }

        void ParseNullAction()
        {
            float.Parse(null);
        }
    }
}
