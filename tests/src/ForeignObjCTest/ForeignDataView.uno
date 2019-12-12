using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace ForeignObjCTest
{
    public extern(FOREIGN_OBJC_SUPPORTED) class DataView
    {
        [Foreign(Language.ObjC)]
        static bool TestContents(ForeignDataView data)
        @{
            auto bytes = (uint8_t*)data.bytes;
            return bytes[0] == 1 && bytes[1] == 2 && bytes[2] == 3;
        @}

        [Test]
        public void Contents()
        {
            Assert.IsTrue(TestContents(ForeignDataView.Create(new byte[] { 1, 2, 3 })));
            Assert.IsTrue(TestContents(ForeignDataView.Create(new sbyte[] { 1, 2, 3 })));
        }

        [Foreign(Language.ObjC)]
        static void Modify(ForeignDataView data)
        @{
            auto bytes = (uint8_t*)data.bytes;
            bytes[1] = 123;
        @}

        [Test]
        public void Modifications()
        {
            var bytes = new byte[] { 1, 2, 3 };
            Modify(ForeignDataView.Create(bytes));
            Assert.AreEqual(123, bytes[1]);
        }
    }
}
