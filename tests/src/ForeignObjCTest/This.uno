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
            NSString* str = @{This:of(_this).Field:get()};
            @{This:of(_this).Field:set([str stringByAppendingString:@"123"])};
            return @{This:of(_this).Field:get()};
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
                @{_staticField:set(@"Test1")};
                return @{_staticField:get()};
            @}

            [Foreign(Language.ObjC)]
            public string Test2()
            @{
                @{_staticField:set(@"Test2")};
                return @{_staticField};
            @}

            [Foreign(Language.ObjC)]
            public string Test3()
            @{
                @{ThisField:of(_this)._someField:set(@"Test3")};
                return @{ThisField:of(_this)._someField:get()};
            @}

            [Foreign(Language.ObjC)]
            public string Test4()
            @{
                @{ThisField:of(_this)._someField:set(@"Test4")};
                return @{ThisField:of(_this)._someField};
            @}

            [Foreign(Language.ObjC)]
            public string Test5()
            @{
                @{ThisField:of(_this)._someProperty:set(@"Test5")};
                return @{ThisField:of(_this)._someProperty:get()};
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
