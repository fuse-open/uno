namespace Mono.test_207
{
    using Uno;

    delegate void Test (int test);

    class X
    {
        static int result = 0;

        public static void hello (int arg)
        {
            result += arg;
        }

        public static void world (int arg)
        {
            result += 16 * arg;
        }

        [Uno.Testing.Test] public static void test_207() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Test a = new Test (hello);
            Test b = new Test (world);

            (a + b) (1);
            if (result != 17)
                return 1;

            ((result == 17) ? a : b) (2);
            if (result != 19)
                return 2;

            ((result == 17) ? a : b) (2);
            if (result != 51)
                return 3;

            return 0;
        }
    }
}
