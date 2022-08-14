using Uno;
using Uno.Testing;

namespace Uno.Test
{
    public class DoubleTest
    {
        [Test]
        public void IsNaN()
        {
            Assert.IsTrue(double.IsNaN(double.NaN));
            Assert.IsTrue(double.IsNaN(.0/.0));
            Assert.IsFalse(double.IsNaN(1.0/.0));
            Assert.IsFalse(double.IsNaN(-1.0/.0));
            Assert.IsFalse(double.IsNaN(1.1));
        }

        [Test]
        public void IsInfinity()
        {
            Assert.IsTrue(double.IsInfinity(double.PositiveInfinity));
            Assert.IsTrue(double.IsInfinity(double.NegativeInfinity));
            Assert.IsFalse(double.IsInfinity(double.NaN));
            Assert.IsFalse(double.IsInfinity(1.1));
        }

        [Test]
        public void IsPositiveInfinity()
        {
            Assert.IsTrue(double.IsPositiveInfinity(double.PositiveInfinity));
            Assert.IsFalse(double.IsPositiveInfinity(double.NegativeInfinity));
            Assert.IsFalse(double.IsPositiveInfinity(double.NaN));
            Assert.IsFalse(double.IsPositiveInfinity(1.1));
        }

        [Test]
        public void IsNegativeInfinity()
        {
            Assert.IsTrue(double.IsNegativeInfinity(double.NegativeInfinity));
            Assert.IsFalse(double.IsNegativeInfinity(double.PositiveInfinity));
            Assert.IsFalse(double.IsNegativeInfinity(double.NaN));
            Assert.IsFalse(double.IsNegativeInfinity(1.1));
        }

        [Test]
        public void TryParse()
        {
            double res;
            Assert.IsTrue(double.TryParse(" +554.2 ", out res));
            Assert.AreEqual(554.2, res);
            Assert.IsFalse(double.TryParse("str", out res));
            Assert.AreEqual(0, res);
        }

        [Test]
        public void ParseValidValue()
        {
            Assert.AreEqual(32.5, double.Parse("32.5"));
            Assert.AreEqual(-55.2, double.Parse("-55.2"));
            Assert.AreEqual(35.2, double.Parse("+35.2"));
            Assert.AreEqual(-7, double.Parse("-7 "));
            Assert.AreEqual(-15.4, double.Parse(" -15.4"));
            Assert.AreEqual(100, double.Parse(" 100 "));
            Assert.AreEqual(0, double.Parse("0"));
        }

        [Test]
        public void ParseInvalidValue()
        {
            Assert.Throws<FormatException>(ParseStringAction);
            Assert.Throws<FormatException>(ParseStringAction2);
            Assert.Throws<FormatException>(ParseEmptyStringAction);
            Assert.Throws<ArgumentNullException>(ParseNullAction);
        }

        void ParseStringAction()
        {
            double.Parse("just_string");
        }

        void ParseStringAction2()
        {
            double.Parse("50foo");
        }

        void ParseEmptyStringAction()
        {
            double.Parse("");
        }

        void ParseNullAction()
        {
            double.Parse(null);
        }

        [Test]
        public void MaxValueToString()
        {
            var maxValueString = double.MaxValue.ToString();
            if defined(CPlusPlus)
            {
                Assert.IsTrue(maxValueString.StartsWith("1797693134862"));
                Assert.AreEqual(309, maxValueString.Length);
            }
            else
            {
                Assert.AreEqual("1.79769313486232E+308", maxValueString);
            }
        }
    }
}
