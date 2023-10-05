using Uno;
using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace ForeignObjCTest
{
    public extern(FOREIGN_OBJC_SUPPORTED) class TypeMacros
    {
        [Foreign(Language.ObjC)]
        [Require("Source.Include", "@{Rect:include}")]
        int typeMacros()
        @{
            @{string} a;
            @{sbyte} b;
            @{Rect} d;
            @{object} e;
            @{int[]} f;
            @{Action} g;
            @{Action<int>} h;
            return 123;
        @}

        [Test]
        public void TestTypeMacros()
        {
            Assert.AreEqual(123, typeMacros());
        }
    }
}
