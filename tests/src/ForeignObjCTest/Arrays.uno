using Uno;
using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace ForeignObjCTest
{
    public extern(FOREIGN_OBJC_SUPPORTED) class Arrays
    {
        [Foreign(Language.ObjC)]
        static int[] primArray(int[] xs)
        @{
            for (int i = 0; i < [xs count]; ++i)
            {
                xs[i] = @( [xs[i] integerValue] + 1 );
            }
            return xs;
        @}

        [Test]
        public void PrimArray()
        {
            var xs = primArray(new int[] { 1, 2, 3 });
            Assert.AreCollectionsEqual(new int[] { 2, 3, 4 }, xs);
        }

        [Foreign(Language.ObjC)]
        static int[] macroArray(int[] xs)
        @{
            for (int i = 0; i < @{int[]:Of(xs).Length:Get()}; ++i)
            {
                int x = @{int[]:Of(xs).Get(i)};
                @{int[]:Of(xs).Set(i, x + 1)};
            }
            return xs;
        @}

        [Test]
        public void MacroArray()
        {
            var xs = macroArray(new int[] { 1, 2, 3 });
            Assert.AreCollectionsEqual(new int[] { 2, 3, 4 }, xs);
        }

        [Foreign(Language.ObjC)]
        static string[] stringArray(string[] xs)
        @{
            /* I'm a comment */
            // Another comment
            for (int i = 0; i < [xs count]; ++i)
            {
                xs[i] = [xs[i] stringByAppendingString:@"abc"];
            }
            return xs;
        @}

        [Test]
        public void StringArray()
        {
            var xs = stringArray(new string[] { "a", "b", "c" });
            Assert.AreCollectionsEqual(new string[] { "aabc", "babc", "cabc" }, xs);
        }

        [Foreign(Language.ObjC)]
        static byte[] byteArray(byte[] xs)
        @{
            /* I'm a comment */
            // Another comment
            for (int i = 0; i < [xs count]; ++i)
            {
                @{byte} x = @{byte[]:Of(xs).Get(i)};
                @{byte[]:Of(xs).Set(i, x + 1)};
            }
            return xs;
        @}

        [Test]
        public void ByteArray()
        {
            var xs = byteArray(new byte[] { 1, 2, 3 });
            Assert.AreCollectionsEqual(new byte[] { 2, 3, 4 }, xs);
        }

        [Foreign(Language.ObjC)]
        static string[] stringArrayCopy(string[] xs)
        @{
            NSArray* xsCopy = [xs copyArray];
            for (int i = 0; i < [xsCopy count]; ++i)
            {
                xs[i] = [xsCopy[i] stringByAppendingString:xsCopy[i]];
            }
            return xs;
        @}

        [Test]
        public void StringArrayCopy()
        {
            var xs = stringArrayCopy(new string[] { "a", "b", "c" });
            Assert.AreCollectionsEqual(new string[] { "aa", "bb", "cc" }, xs);
        }

        class MyObject
        {
            public int Field;
        }

        [Foreign(Language.ObjC)]
        static MyObject[] objectArray(MyObject[] xs)
        @{
            /* I'm a comment */
            // Another comment
            for (int i = 0; i < [xs count]; ++i)
            {
                @{MyObject:Of((@{MyObject})xs[i]).Field:Set(123)};
            }
            return xs;
        @}

        [Test]
        public void ObjectArray()
        {
            var xs = objectArray(new MyObject[] { new MyObject(), new MyObject() });
            foreach (var x in xs)
            {
                Assert.AreEqual(123, x.Field);
            }
        }

        [Foreign(Language.ObjC)]
        static int[][] arrayArray(int[][] xs)
        @{
            for (int i = 0; i < [xs count]; ++i)
            for (int j = 0; j < [xs[0] count]; ++j)
            {
                xs[i][j] = @123;
            }
            return xs;
        @}

        [Test]
        public void ArrayArray()
        {
            var xs = arrayArray(new int[][] { new int[3], new int[3] });
            foreach (var ys in xs)
            foreach (var x in ys)
            {
                Assert.AreEqual(123, x);
            }
        }

        [Foreign(Language.ObjC)]
        static ObjC.Object[] objCObjArray(ObjC.Object[] xs)
        @{
            /* I'm a comment */
            // Another comment
            for (int i = 0; i < [xs count]; ++i)
            {
                /* I'm a comment */
                // Another comment
                xs[i] = [@"aaa" stringByAppendingString:xs[i]];
            }
            return xs;
        @}

        [Foreign(Language.ObjC)]
        ObjC.Object newString(string s) @{ return s; @}
        [Foreign(Language.ObjC)]
        string asString(ObjC.Object s) @{ return s; @}

        [Test]
        public void ObjCObjectArray()
        {
            var xs = objCObjArray(new ObjC.Object[] { newString("abc"), newString("123") });
            Assert.AreEqual("aaaabc", asString(xs[0]));
            Assert.AreEqual("aaa123", asString(xs[1]));
        }

        [Foreign(Language.ObjC)]
        static Func<int, int>[] delegateArray(Func<int, int>[] xs, out int result)
        @{
            *result = 0;
            for (int i = 0; i < [xs count]; ++i)
            {
                *result += ((::uObjC::Function<int, int>)xs[i])(i);
            }
            return xs;
        @}

        int MyFunc1(int arg)
        {
            return arg + arg;
        }

        int MyFunc2(int arg)
        {
            return arg * arg;
        }

        [Test]
        public void DelegateArray()
        {
            int result = 0;
            var xs = delegateArray(new Func<int, int>[] { MyFunc1, MyFunc2, MyFunc1, MyFunc2 }, out result);
            Assert.AreEqual(0 + 0 + 1 * 1 + 2 + 2 + 3 * 3, result);
        }
    }
}
