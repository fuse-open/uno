using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    using Uno.Math;
    using Uno.Vector;

    public class UsingStatic
    {
        class Class1
        {
            public Class1()
            {
            }

            int Clamp(int x, int min, int max)
            {
                return 6;
            }

            float2 Abs(float2 a, int b)
            {
                return float2(-1);
            }

            public void Foo()
            {
                Assert.AreEqual(6, Clamp(0, 0, 0));
                Assert.AreEqual(0, Math.Clamp(0, 0, 0));
                Assert.AreEqual(float2(2.0f), Math.Abs(float2(-2.0f)));
                Assert.AreEqual(float2(-1), Abs(float2(6), 5));
            }
        }

        class Class2
        {
            public Class2()
            {
            }

            float2 Abs(float2 a)
            {
                return float2(-a.X, -a.Y);
            }

            float2 Abs(int a)
            {
                return float2((float)a, (float)-a);
            }

            public void Foo()
            {
                Assert.IsTrue(Abs(float2(1)) == float2(-1));
                Assert.IsTrue(Abs(float2(-6.2f)) == float2(6.2f, 6.2f));
                Assert.IsTrue(Math.Abs(float2(1)) == float2(1));
                Assert.AreEqual(Math.Abs(float2(-6)), float2(6.0f));
            }
        }

        [Test]
        public void Run()
        {
            var i1 = new Class1();
            i1.Foo();

            var i2 = new Class2();
            i2.Foo();

            Assert.IsTrue(Clamp(2.0f, 3.0f, 4.0f) == 3.0f);
            Assert.IsTrue(Abs(6.0f) == 6.0f);
            Assert.IsTrue(Abs(float2(-1, -2)) == float2(1, 2));
        }
    }
}
