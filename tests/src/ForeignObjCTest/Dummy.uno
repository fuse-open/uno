using Uno.Testing;
namespace ForeignObjCTest
{
    public extern(!FOREIGN_OBJC_SUPPORTED) class Dummy
    {
        [Test]
        public void Test()
        {
            Assert.AreEqual(0, 0);
        }
    }
}
