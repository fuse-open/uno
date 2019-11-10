using Uno;
using Uno.IO;
using Uno.Testing;

namespace UnoCore_Uno_IO
{
    public class BundleTests
    {
        [Test]
        public static void LoadUtf8Bom()
        {
            var text = import("TestData/utf8-bom.txt").ReadAllText();
            Assert.AreEqual(text, "hello");
        }
    }
}
