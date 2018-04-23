using Uno.Testing;

namespace Uno.UX.Tests
{
    public class PropertyTests
    {
        [Test]
        public void DependencyBasics()
        {
            var e = new UXHelpers.PropertyBasics();
            Assert.AreEqual(1337.0f, e.Foo.MyFloat);
        }
    }
}
