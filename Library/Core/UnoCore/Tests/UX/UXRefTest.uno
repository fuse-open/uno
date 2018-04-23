using Uno;
using Uno.Testing;

namespace Uno.UX.Tests
{
    

    public class UXRefTests
    {
        [Test]
        public void TestUXRef()
        {
            var e = new UXHelpers.RefTest();

            // The ux:Test node should not be added
            foreach (var c in e.Children)
                Assert.AreNotEqual("lol", e.Name);

            // Check that the ux:Ref node was added correctly
            Assert.AreEqual(e.refPanel.innerPanel.Children[0], e.p1);
        }

    }
}

