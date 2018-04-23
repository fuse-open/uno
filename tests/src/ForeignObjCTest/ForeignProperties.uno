using Uno;
using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace ForeignObjCTest
{
    public extern(FOREIGN_OBJC_SUPPORTED) class ForeignProperties
    {
        public class ForeignPropClass
        {
            int _foo;

            [Foreign(Language.ObjC)]
            public int Foo
            {
                get
                @{
                    return @{ForeignPropClass:Of(_this)._foo};
                @}
                set
                @{
                    @{ForeignPropClass:Of(_this)._foo:Set(value)};
                @}
            }

            string _bar;

            [Foreign(Language.ObjC)]
            public string Bar
            {
                get
                @{
                    return @{ForeignPropClass:Of(_this)._bar};
                @}
                set
                @{
                    @{ForeignPropClass:Of(_this)._bar:Set(value)};
                @}
            }
        }

        [Test]
        public void GetSetProperties()
        {
            var pc = new ForeignPropClass();

            pc.Foo = 123;
            pc.Bar = "abc 123";

            Assert.AreEqual(123, pc.Foo);
            Assert.AreEqual("abc 123", pc.Bar);
        }
    }
}
