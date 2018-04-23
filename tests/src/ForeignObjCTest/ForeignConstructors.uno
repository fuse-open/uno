using Uno;
using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace ForeignObjCTest
{
    public extern(FOREIGN_OBJC_SUPPORTED) class ForeignConstructors
    {
        public class FCBase
        {
            protected int Proof1 = 20;
            protected string Proof2;
            public FCBase()
            {
                Proof2 = "PROOF2";
            }
        }

        public class ForeignConstructorClass : FCBase
        {
            string Proof3;

            [Foreign(Language.ObjC)]
            public ForeignConstructorClass()
            @{
                // inside the foreign constructor
                @{ForeignConstructorClass:Of(_this).Proof3:Set(@"PROOF3")};
            @}

            public void TestForeignConstructor()
            {
                Assert.AreEqual(20, Proof1);
                Assert.AreEqual("PROOF2", Proof2);
                Assert.AreEqual("PROOF3", Proof3);
            }
        }

        [Test]
        public void ForeignConstructor()
        {
            var o = new ForeignConstructorClass();
            o.TestForeignConstructor();
        }

        public static class ForeignStaticConstructorClass
        {
            static string _test = "abc";
            [Foreign(Language.ObjC)]
            static ForeignStaticConstructorClass()
            @{
                NSString* str = @{_test};
                @{_test:Set(@"haaa")};
            @}

            public static void TestForeignConstructor()
            {
                Assert.AreEqual("haaa", _test);
            }
        }

        [Test]
        public void ForeignStaticConstructor()
        {
            ForeignStaticConstructorClass.TestForeignConstructor();
        }
    }
}
