using Uno;
using Uno.Testing;

namespace Uno.Test
{
    public class BoolTest
    {
        [Test]
        public void TryParse()
        {
            bool res;
            Assert.IsTrue(bool.TryParse("true", out res));
            Assert.IsTrue(res);
            Assert.IsFalse(bool.TryParse("str", out res));
            Assert.IsFalse(res);
            Assert.IsFalse(bool.TryParse("15", out res));
            Assert.IsFalse(res);
        }

        [Test]
        public void ParseValidValue()
        {
            Assert.AreEqual(false, bool.Parse("False"));
            Assert.AreEqual(false, bool.Parse("false"));
            Assert.AreEqual(false, bool.Parse("FALSE"));
            Assert.AreEqual(false, bool.Parse(" FALSE "));
            Assert.AreEqual(true, bool.Parse("True"));
            Assert.AreEqual(true, bool.Parse("TRUE"));
            Assert.AreEqual(true, bool.Parse("true"));
            Assert.AreEqual(true, bool.Parse(" true "));
        }

        [Test]
        public void ParseInvalidValue()
        {
            Assert.Throws<FormatException>(ParseStringAction);
            Assert.Throws<FormatException>(ParseEmptyStringAction);
            Assert.Throws<ArgumentNullException>(ParseNullAction);
        }

        void ParseStringAction()
        {
            bool.Parse("just_string");
        }

        void ParseEmptyStringAction()
        {
            bool.Parse("");
        }

        void ParseNullAction()
        {
            bool.Parse(null);
        }
    }
}
