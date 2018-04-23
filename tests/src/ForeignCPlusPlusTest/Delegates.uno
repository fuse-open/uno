using Uno;
using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace ForeignCPlusPlusTest
{
    [Foreign(Language.CPlusPlus)]
    delegate void MyAction();

    [Foreign(Language.CPlusPlus)]
    delegate int MyPrimitiveDelegate(int x, long y, char z);

    [Foreign(Language.CPlusPlus)]
    delegate int MyStringDelegate(string str);

    [Foreign(Language.CPlusPlus)]
    delegate int MyArrayDelegate(byte[] buf, int bufLength);

    [Foreign(Language.CPlusPlus)]
    delegate int MyMultiArrayDelegate(byte[] buf, int bufLength, int[] buf2, int buf2Length);
    [Foreign(Language.CPlusPlus)]
    delegate void MyOutParamDelegate(out int outInt, ref bool outBool);

    public extern(CPlusPlus || PInvoke || ENABLE_CIL_TESTS) class DelegateTests
    {
        [Foreign(Language.CPlusPlus)]
        static void UseAction(MyAction f)
        @{
            @{MyAction} g = f;
            @{MyAction:Of(g):Call()};
        @}

        bool _ran;

        void AnAction()
        {
            _ran = true;
        }

        [Test]
        public void VoidDelegate()
        {
            _ran = false;
            UseAction(AnAction);
            Assert.IsTrue(_ran);
        }

        [Foreign(Language.CPlusPlus)]
        static int UsePrimitiveDelegate(MyPrimitiveDelegate f)
        @{
            @{MyPrimitiveDelegate} g = f;
            auto result = @{MyPrimitiveDelegate:Of(g):Call(123, 456, 23)};
            return result;
        @}

        int APrimitiveDelegate(int x, long y, char z)
        {
            _ran = true;
            return x + (int)y + (int)z;
        }

        [Test]
        public void PrimitiveDelegate()
        {
            _ran = false;
            var result = UsePrimitiveDelegate(APrimitiveDelegate);
            Assert.IsTrue(_ran);
            Assert.AreEqual(123 + 456 + 23, result);
        }

        [Foreign(Language.CPlusPlus)]
        static int UseStringDelegate(MyStringDelegate f)
        @{
            @{MyStringDelegate} g = f;
            @{int} len = 5;
            @{char} str[6];
            str[5] = 0;
            for (@{int} i = 0; i < len; ++i)
                str[i] = (@{char})('a' + i);
            auto result = @{MyStringDelegate:Of(g):Call(str)};
            return result;
        @}

        int AStringDelegate(string str)
        {
            _ran = true;
            Assert.AreEqual(5, str.Length);
            for (int i = 0; i < str.Length; ++i)
                Assert.AreEqual((char)((int)'a' + i), str[i]);
            return 123;
        }

        [Test]
        public void StringDelegate()
        {
            _ran = false;
            var result = UseStringDelegate(AStringDelegate);
            Assert.IsTrue(_ran);
            Assert.AreEqual(123, result);
        }

        [Foreign(Language.CPlusPlus)]
        static int UseArrayDelegate(MyArrayDelegate f)
        @{
            @{MyArrayDelegate} g = f;
            @{int} len = 5;
            @{byte} buf[5];
            for (@{int} i = 0; i < len; ++i)
                buf[i] = (@{byte})i;
            auto result = @{MyArrayDelegate:Of(g):Call(buf, len)};
            return result;
        @}

        int AnArrayDelegate(byte[] buf, int len)
        {
            _ran = true;
            Assert.AreEqual(5, len);
            Assert.AreEqual(len, buf.Length);
            for (int i = 0; i < len; ++i)
                Assert.AreEqual(i, buf[i]);
            return 123;
        }

        [Test]
        public void ArrayDelegate()
        {
            _ran = false;
            var result = UseArrayDelegate(AnArrayDelegate);
            Assert.IsTrue(_ran);
            Assert.AreEqual(123, result);
        }

        [Foreign(Language.CPlusPlus)]
        static int UseMultiArrayDelegate(MyMultiArrayDelegate f)
        @{
            @{MyMultiArrayDelegate} g = f;
            @{int} bufLen = 5;
            @{byte} buf[5];
            for (@{int} i = 0; i < bufLen; ++i)
                buf[i] = (@{byte})i;
            @{int} buf2Len = 10;
            @{int} buf2[10];
            for (@{int} i = 0; i < buf2Len; ++i)
                buf2[i] = i * i;
            auto result = @{MyMultiArrayDelegate:Of(g):Call(buf, bufLen, buf2, buf2Len)};
            return result;
        @}

        int AMultiArrayDelegate(byte[] buf, int bufLen, int[] buf2, int buf2Len)
        {
            _ran = true;
            Assert.AreEqual(5, bufLen);
            Assert.AreEqual(10, buf2Len);
            Assert.AreEqual(bufLen, buf.Length);
            Assert.AreEqual(buf2Len, buf2.Length);
            for (int i = 0; i < bufLen; ++i)
                Assert.AreEqual(i, buf[i]);
            for (int i = 0; i < buf2Len; ++i)
                Assert.AreEqual(i * i, buf2[i]);
            return 123;
        }

        [Test]
        public void MultiArrayDelegate()
        {
            _ran = false;
            var result = UseMultiArrayDelegate(AMultiArrayDelegate);
            Assert.IsTrue(_ran);
            Assert.AreEqual(123, result);
        }

        [Foreign(Language.CPlusPlus)]
        static int UseOutParamDelegate(MyOutParamDelegate f)
        @{
            @{MyOutParamDelegate} g = f;
            @{int} x1;
            @{bool} y1 = true;
            @{MyOutParamDelegate:Of(g):Call(&x1, &y1)};
            @{int} x2;
            @{bool} y2 = false;
            @{MyOutParamDelegate:Of(g):Call(&x2, &y2)};
            return (y1 ? x1 : -x1) * (y2 ? x2 : -x2);
        @}

        void AnOutParamDelegate(out int x, ref bool y)
        {
            _ran = true;
            x = 5;
            if (y)
            {
                x = 7;
            }
            y = !y;
        }

        [Test]
        public void OutParamDelegate()
        {
            _ran = false;
            var result = UseOutParamDelegate(AnOutParamDelegate);
            Assert.IsTrue(_ran);
            Assert.AreEqual(-7 * 5, result);
        }
    }
}
