using Uno;
using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace ForeignObjCTest
{
    public extern(FOREIGN_OBJC_SUPPORTED) class UnoObjects
    {
        public class MyClass
        {
            public string Field;

            public string Function(string a)
            {
                return a + Field;
            }

            public void MyAction()
            {
            }

            [Foreign(Language.ObjC)]
            public MyClass(string i)
            @{
                @{MyClass:Of(_this).Field:Set(i)};
            @}
        }

        public struct MyStruct
        {
            public int Field;

            public MyStruct(int i)
            {
                Field = i;
            }
        }

        [Foreign(Language.ObjC)]
        static MyClass objReturn(MyClass o)
        @{
            return o;
        @}

        [Test]
        public void ObjReturn()
        {
            var o = new MyClass("abc 123");
            Assert.AreEqual(o, objReturn(o));
        }

        [Foreign(Language.ObjC)]
        public static MyStruct structReturn(MyStruct s)
        @{
            return s;
        @}

        [Test]
        public void StructReturn()
        {
            var s = new MyStruct(123);
            Assert.AreEqual(123, structReturn(s).Field);
        }

        [Foreign(Language.ObjC)]
        static MyStruct structCreate()
        @{
            return @{MyStruct(int):New(123)};
        @}

        [Test]
        public void StructCreate()
        {
            Assert.AreEqual(123, structCreate().Field);
        }

        [Foreign(Language.ObjC)]
        static MyClass objectCreate()
        @{
            NSString *str = @"aaabbb";
            return @{MyClass(string):New(str)}; // comment
        @}

        [Test]
        public void MacroNew()
        {
            Assert.AreEqual("aaabbb", objectCreate().Field);
        }

        [Foreign(Language.ObjC)]
        static string macroCall(MyClass o)
        @{
            NSString *str = @"xxx"; // comment
            return @{MyClass:Of(o).Function(string):Call(str)};
        @}

        [Test]
        public void MacroCall()
        {
            Assert.AreEqual("xxxabc123", macroCall(new MyClass("abc123")));
        }

        [Foreign(Language.ObjC)]
        static string macroSetGet(MyClass o)
        @{
            NSString* str = @"Hello!";
            @{MyClass:Of(o).Field:Set(str)}; // comment
            return @{MyClass:Of(o).Field:Get()};
        @}

        [Test]
        public void MacroSetGet()
        {
            Assert.AreEqual("Hello!", macroSetGet(new MyClass("abc123")));
        }

        [Foreign(Language.ObjC)]
        static string macroField(MyClass o)
        @{
            NSString* str = @"Hello!";
            @{MyClass:Of(o).Field:Set(str)}; // comment
            return @{MyClass:Of(o).Field};
        @}

        [Test]
        public void MacroField()
        {
            Assert.AreEqual("Hello!", macroSetGet(new MyClass("abc123")));
        }
    }
}
