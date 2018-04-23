using Uno;
using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace ForeignObjCTest
{
    // TODO
    public extern(FOREIGN_OBJC_SUPPORTED) class This
    {

        string Field = "abc";

        [Foreign(Language.ObjC)]
        string useThis()
        @{
            /* I'm a comment */
            // Another comment
            NSString* str = @{This:Of(_this).Field:Get()};
            @{This:Of(_this).Field:Set([str stringByAppendingString:@"123"])};
            return @{This:Of(_this).Field:Get()};
            /* I'm a comment */
            // Another comment
        @}

        [Test]
        public void UseThis()
        {
            Assert.AreEqual("abc123", useThis());
        }

        class ThisField
        {
            public static string _staticField;
            public string _someField;
            public string _someProperty { get; set; }

            public ThisField()
            {
            }

            [Foreign(Language.ObjC)]
            public string Test1()
            @{
                @{_staticField:Set(@"Test1")};
                return @{_staticField:Get()};
            @}

            [Foreign(Language.ObjC)]
            public string Test2()
            @{
                @{_staticField:Set(@"Test2")};
                return @{_staticField};
            @}

            [Foreign(Language.ObjC)]
            public string Test3()
            @{
                @{ThisField:Of(_this)._someField:Set(@"Test3")};
                return @{ThisField:Of(_this)._someField:Get()};
            @}

            [Foreign(Language.ObjC)]
            public string Test4()
            @{
                @{ThisField:Of(_this)._someField:Set(@"Test4")};
                return @{ThisField:Of(_this)._someField};
            @}

            [Foreign(Language.ObjC)]
            public string Test5()
            @{
                @{ThisField:Of(_this)._someProperty:Set(@"Test5")};
                return @{ThisField:Of(_this)._someProperty:Get()};
            @}
        }

        [Test]
        public void UseThisToo()
        {
            var o = new ThisField();
            Assert.AreEqual("Test1", o.Test1());
            Assert.AreEqual("Test2", o.Test2());
            Assert.AreEqual("Test3", o.Test3());
            Assert.AreEqual("Test4", o.Test4());
            Assert.AreEqual("Test5", o.Test5());
        }
    }
}
