using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class StaticInitialization2
    {
        [Test]
        public void Run()
        {
            // Referencing these entities would trigger a crash during static
            // initialization on native targets. See
            // https://github.com/fusetools/uno/issues/1631.
            Assert.AreEqual(0, string.Compare("foo", "foo"));
            Assert.AreEqual(0.5, Math.Round(0.51, 1));
        }
    }
}
