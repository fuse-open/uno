using Uno;
using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace SwiftFileIncludeTest
{
    public extern(iOS) class SimpleFileInclude
    {
        [Foreign(Language.ObjC)]
        [ForeignInclude(Language.ObjC, "@(project.name)-Swift.h")]
        string GetTheString()
        @{
            HelloSwiftWorld* x = [[HelloSwiftWorld alloc] init];
            NSString* result = [x hello];
            return result;
        @}

        [Test]
        public void GetSwiftString()
        {
            Assert.AreEqual("Hello Swift world!", GetTheString());
        }
    }

    public extern(!iOS) class Dummy
    {
        [Test]
        public void Test()
        {
            Assert.AreEqual(0, 0);
        }
    }
}
