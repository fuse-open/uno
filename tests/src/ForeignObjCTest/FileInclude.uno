using Uno;
using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace ForeignObjCTest
{
    public extern(FOREIGN_OBJC_SUPPORTED) class FileInclude
    {
        [Foreign(Language.ObjC)]
        [ForeignInclude(Language.ObjC, "FileInclude.h")]
        string StringFromFileInclude() @{ return string_from_file_include(); @}

        [Test]
        public void FileIncludeTest()
        {
            Assert.AreEqual("lollercoaster tycoon", StringFromFileInclude());
        }
    }
}
