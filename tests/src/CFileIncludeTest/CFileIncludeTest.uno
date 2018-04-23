using Uno.Testing;
using Uno.Compiler.ExportTargetInterop;

namespace CFileIncludeTest
{
    public extern(CPlusPlus) class SimpleFileInclude
    {
        [Test]
        [Require("Source.Include", "example.h")]
        public void GetCString()
        {
            var str = extern<string> "uString::Ansi(hello_world())";
            Assert.AreEqual("Hello from C++", str);
        }
    }

    public extern(!CPlusPlus) class Dummy
    {
        [Test]
        public void Test()
        {
            Assert.AreEqual(0, 0);
        }
    }
}
