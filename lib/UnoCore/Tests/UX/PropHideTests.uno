using Uno.Testing;

namespace Uno.UX.Tests
{
    public class PropHideTests
    {
        [Test]
        public void PropHide()
        {
            var ph = new UXHelpers.PropHide();
            Assert.AreEqual("Hello", ph.Phc.X);
        }
    }
}