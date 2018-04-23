using Uno;
using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace ForeignObjCTest
{
    public extern(FOREIGN_OBJC_SUPPORTED) class Delegates
    {
        public delegate double DoubleFun(int i);

        [Foreign(Language.ObjC)]
        static extern(FOREIGN_OBJC_SUPPORTED) double CallDoubleFun(DoubleFun f)
        @{
            /* I'm a comment */
            // Another comment
            return f(12);
        @}

        static double MyDoubleFun(int i)
        {
            return (double)i + (double)i;
        }

        [Test]
        public void PrimitiveDelegate()
        {
            Assert.AreEqual((double)12 + (double)12, CallDoubleFun(MyDoubleFun));
        }

        [Foreign(Language.ObjC)]
        string CallStringFunc(Func<string, string> f)
        @{
            return [f(@"abc 123") stringByAppendingString:f(@"lol")];
        @}

        static string MyStringFun(string s)
        {
            return s + s;
        }

        [Test]
        public void StringFunc()
        {
            Assert.AreEqual("abc 123abc 123lollol", CallStringFunc(MyStringFun));
        }


        [Foreign(Language.ObjC)]
        Func<string, int, object> ReturnObjectFunc(Func<string, int, object> f)
        @{
            /* I'm a comment */
            // Another comment
            return f;
        @}

        public static object MyObjectFunc(string s, int n)
        {
            return s + n;
        }

        [Test]
        void ObjectFunc()
        {
            Assert.AreEqual("abc123", ReturnObjectFunc(MyObjectFunc)("abc", 123));
        }

        [Foreign(Language.ObjC)]
        static void CallActionInt(Action<int> f)
        @{
            f(1230);
        @}

        bool _called;

        void MyActionInt(int i)
        {
            _called = true;
            Assert.AreEqual(i, 1230);
        }

        [Test]
        public void ActionInt()
        {
            _called = false;
            CallActionInt(MyActionInt);
            Assert.IsTrue(_called);
        }

        [Foreign(Language.ObjC)]
        static Action ForeignReturnAction(Action f)
        @{
            return f;
            /* I'm a comment */
            // Another comment
        @}

        void MyAction()
        {
            _called = true;
        }

        [Test]
        public void ReturnAction()
        {
            _called = false;
            ForeignReturnAction(MyAction)();
            Assert.IsTrue(_called);
        }


        [Foreign(Language.ObjC)]
        static Func<int, int> FuncReturn()
        @{
            /* I'm a comment */
            // Another comment
            return ^ (int i) { return i + i; };
        @}

        [Test]
        public void CallFuncReturn()
        {
            Assert.AreEqual(123 + 123, FuncReturn()(123));
        }

        [Foreign(Language.ObjC)]
        static double macroDelegate(Func<int, double> f)
        @{
            double g = @{Func<int, double>:Of(f):Call(12)};
            return g;
        @}

        double MyFunc(int x)
        {
            return (double)(x + x);
        }

        [Test]
        public void MacroDelegate()
        {
            var y = macroDelegate(MyFunc);
            Assert.AreEqual((double)(12 + 12), y);
        }
    }
}
