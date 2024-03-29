namespace Mono.test_727
{
    using Uno;

    namespace IDisposableTest
    {
        class MainClass
        {
            [Uno.Testing.Test] public static void test_727() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
                using (Foo f = new Foo ())
                    ;

                Console.WriteLine ("Between. Foo.TotalInstances = " + Foo.TotalInstances);

                using (IDisposable f = new Foo ())
                    ;

                Console.WriteLine ("After. Foo.TotalInstances = " + Foo.TotalInstances);

                if (Foo.TotalInstances != 2)
                    return 1;

                return 0;
            }
        }


        class Foo : IDisposable
        {
            public static int TotalInstances = 0;

            private int my_a = 0;

            public Foo ()
            {
                my_a = TotalInstances++;
                Console.WriteLine ("Instance " + my_a + " ctor");
            }

            public void Dispose ()
            {
                Console.WriteLine ("Instance " + my_a + " Dispose()");
            }
        }

    }
}
