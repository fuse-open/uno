using Uno.Testing;

namespace Uno.UX.Tests
{
    public class DependencyTests
    {
        [Test]
        public void DependencyBasics()
        {
            var e = new UXHelpers.DependencyBasics();
            Assert.AreEqual(1337.0f, e.Foo.ReadHelper.Object);
        }
    }
}
