using Uno;
using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace ForeignObjCTest
{
    public extern(FOREIGN_OBJC_SUPPORTED) class Names
    {
        public class NameTest
        {
            int aName;

            [Foreign(Language.ObjC)]
            public int Method(int aName)
            @{
                return aName;
            @}
        }

        [Test]
        public void FieldClash()
        {
            Assert.AreEqual(123, new NameTest().Method(123));
        }
    }
}
