using Uno.Testing;

namespace ForeignJavaTest
{
    public extern(!android) class Dummy
    {
        [Test]
        public void Test()
        {
            Assert.AreEqual(0, 0);
        }
    }
}
