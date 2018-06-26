using Uno;
using Uno.Testing;

namespace UnoExtensions
{
    public static class StringExtensions
    {
        public static string Hello(this string self)
        {
            return "Hello, " + self;
        }
        /*public static int LengthSquared(this string self)
        {
            get { return self.Length * self.Length; }
        }*/
    }
}

namespace UnoTest.General
{
    using UnoExtensions;

    public class ExtensionMethods
    {
        [Test]
        public void Run()
        {
            Assert.AreEqual("Hello, foo", "foo".Hello());
            Assert.AreEqual("Hello, foo", StringExtensions.Hello("foo"));

            //Assert.AreEqual(9, "foo".LengthSquared);
            //Assert.AreEqual(9, StringExtensions.LengthSquared("foo"));
        }
    }
}
