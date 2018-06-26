using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class TernaryOps
    {
        class Foo
        {
        }

        class FooWrapper
        {
            Foo _foo;

            public Foo Foo
            {
                get { return _foo ?? (Foo = new Foo()); }
                set { _foo = value; }
            }

            public Foo FooSimple
            {
                get { return _foo ?? (_foo = new Foo()); }
                set { _foo = value; }
            }
        }

        [Test]
        public void Run()
        {
            float x = 1.0f;
            float s = x != 0.0f ? Math.Sin(x) / x : 1.0f;

            int a = 5, b = 10, c = 3, d = 2, e = 7;

            int min = a < b ? a : b;
            Assert.IsTrue(min ==a);

            int lol = a == 3 ? b : c == 3 ? d : e;
            Assert.IsTrue(lol == d);

            var cam = ObjectThingie.Current ?? ObjectThingie.Default;
            Assert.IsFalse(cam == ObjectThingie.Default);

            ObjectThingie cam2 = null;
            cam = cam2 ?? ObjectThingie.Current;
            Assert.IsTrue(cam == ObjectThingie.Current);

            var derp = "derp";
            Assert.IsTrue((true ? derp : null) == derp);
            Assert.IsTrue((false ? null : derp) == derp);
            Assert.IsTrue((null ?? derp) == derp);
            Assert.IsTrue((derp ?? null) == derp);

            Assert.IsTrue(new FooWrapper().Foo != null);
            Assert.IsTrue(new FooWrapper().FooSimple != null);

            // This is not allowed
            //Assert.IsTrue(new FooWrapper() == new Foo());
        }

        class ObjectThingie
        {
            static ObjectThingie current;
            public static ObjectThingie Current
            {
                get
                {
                    return current;
                }
                set
                {
                    current = value;
                }
            }
            public static ObjectThingie Default
            {
                get
                {
                    return new ObjectThingie();
                }
            }

            // This is not allowed
            /*
            public string Foo
            {
                get; set {}
            }
            */

            // This is not allowed
            /*
            public string Bar
            {
                get {} set;
            }
            */
        }
    }
}
