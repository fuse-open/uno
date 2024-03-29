namespace Mono.test_384
{
    using Uno;

    class X
    {
        static int Foo = 10;

        static void Test ()
        {
            while (true) {
                if (Foo == 1)
                    throw new Exception ("Error Test");
                else
                    break;
            }

            Foo = 20;
        }

        [Uno.Testing.Test] public static void test_384() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Test ();
            if (Foo != 20)
                return 1;
            return 0;
        }
    }
}
