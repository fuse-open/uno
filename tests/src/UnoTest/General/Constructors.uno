using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class Constructors
    {
        class Class
        {
            int foo;

            Class(int foo)
            {
                this.foo = foo;
            }

            public Class()
                : this(1)
            {
            }

            public int GetFoo()
            {
                return foo;
            }
        }

        struct Struct
        {
            int foo;
            int temp;

            Struct(int foo, int bar)
                : this()
            {
                this.foo = foo;
            }

            public Struct(int bar)
                : this(1, bar)
            {
            }

            public Struct(int a, int b, int c)
            {
                this = new Struct(1);
            }

            public int GetFoo()
            {
                return foo;
            }

            public void Replace()
            {
                this = new Struct(5, 5);
            }
        }

        public abstract class Foo
        {
            public class Bar : Foo
            {
                public Bar()
                {
                }
            }

            private static Bar BarImpl = new Bar();
        }

        [Test]
        public void Run()
        {
            var c = new Class();
            var s = new Struct(5);

            Assert.AreEqual(1, c.GetFoo());
            Assert.AreEqual(1, s.GetFoo());

            s.Replace();
            Assert.AreEqual(5, s.GetFoo());

            s = new Struct(1, 2, 3);
            Assert.AreEqual(1, s.GetFoo());
        }
    }
}
