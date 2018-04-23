using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class Base
    {
        abstract class A
        {
            public int I = 1;
            public int J = 2;

            public abstract int Foo();
            public virtual int Bar() { return J; }
        }

        class B : A
        {
            public int I = 2;
            public int P { get { return 5; } }
            public virtual int Q { get { return 5; } }

            public override int Foo() { return base.I; }
            public override int Bar() { return base.Bar() + I; }
        }

        class C : B
        {
            public int P { get { return 7; } }
            public override int Q { get { return 2; } }

            public override int Foo() { return base.Foo() + base.P; }
            public override int Bar() { return base.Foo() + base.Q; }

            public C()
            {
                var p = P;
                var q = Q;
            }
        }

        class D : A
        {
            public override int Foo()
            {
                return 2;
            }
        }

        [Test]
        public void Run()
        {
            var b = new B();
            var c = new C();
            var d = new D();

            Assert.AreEqual(1, b.Foo());
            Assert.AreEqual(4, b.Bar());
            Assert.AreEqual(6, c.Foo());
            Assert.AreEqual(6, c.Bar());
            Assert.AreEqual(2, d.Foo());
            Assert.AreEqual(2, d.Bar());
        }
    }
}
