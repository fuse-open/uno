using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class Call
    {
        void DefVal(int i = 10)
        {
            Assert.AreEqual(10, i);
        }

        void Params(int i = 0, params float[] floats)
        {
            Assert.AreEqual(floats.Length, i);
        }

        void NoParams(int i, float[] floats)
        {
            Assert.AreEqual(floats.Length, i);
        }

        delegate void ParamsDelegate1(int i = 0, params float[] floats);
        delegate void ParamsDelegate2(int i, params float[] floats);
        delegate void ParamsDelegate3(int i, float[] floats);

        int Params2_0_called;
        int Params2_1_called;
        int Params2_2_called;

        void Params2()
        {
            Params2_0_called++;
        }

        void Params2(int i = 1)
        {
            Params2_1_called++;
            Assert.AreEqual(0, i);
        }

        void Params2(int i = 0, params float[] floats)
        {
            Params2_2_called++;
            Assert.AreEqual(floats.Length, i);
        }

        class IntWrapper
        {
            public int i;

            public IntWrapper(int v = 4)
            {
                i = v;
            }

            public static implicit operator int(IntWrapper c)
            {
                return c.i;
            }
        }

        bool Foo(int i)
        {
            return false;
        }

        bool Foo<T>(int i)
        {
            return true;
        }

        string ResolveParams(object obj) { return "object"; }
        string ResolveParams(params object[] objs) { return "params"; }

        static string f(float f) { return "float";}
        static string f<T>(T f) { return "T";}

        static string g(int i) { return "int"; }
        static string g(float f) { return "float";}
        static string g<T>(T f) { return "T";}

        [Test]
        public void Run()
        {
            DefVal();

            Params();
            Params(0);
            Params(1, 2);
            Params(2, 3, 4);
            Params(3, 4, 5, 6);

            ParamsDelegate1 del1 = Params;
            ParamsDelegate1 del2 = NoParams;
            ParamsDelegate2 del3 = Params;
            ParamsDelegate3 del4 = Params;

            del1();
            del2();
            del3(0);
            del3(1, 2);
            del4(0, new float[0]);

            Params2();
            Params2(0);
            Params2(1, 2);

            Assert.AreEqual(1, Params2_0_called);
            Assert.AreEqual(1, Params2_1_called);
            Assert.AreEqual(1, Params2_2_called);

            ulong ulongCastViaInt = (ulong)new IntWrapper();
            Assert.AreEqual(4, ulongCastViaInt);

            var sumViaInt = new IntWrapper() + new IntWrapper();
            Assert.AreEqual(8, sumViaInt);

            Assert.IsFalse(Foo(10));
            Assert.IsTrue(Foo<bool>(10));

            Assert.AreEqual("object", ResolveParams(1));
            Assert.AreEqual("T", f(1));
            Assert.AreEqual("int", g(1));
        }
    }
}
