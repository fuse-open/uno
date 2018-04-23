using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace ForeignObjCTest
{
    public extern(FOREIGN_OBJC_SUPPORTED) class TypeName
    {
        [ForeignTypeName("::NSURL*")]
        public class NSURL : ObjC.Object { }

        [Foreign(Language.ObjC)]
        public static NSURL CreateNSURL(string path)
        @{
            return [NSURL fileURLWithPath:path];
        @}

        [Foreign(Language.ObjC)]
        public static string UseNSURL(NSURL url)
        @{
            return url.absoluteString;
        @}

        [Test]
        public void NSURLTest()
        {
                Assert.AreEqual("file:///path1/path2.ext", UseNSURL(CreateNSURL("/path1/path2.ext")));
        }
    }
}
