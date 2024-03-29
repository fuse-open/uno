namespace Mono.test_189
{
    //
    // Test to ensure proper overload resolution with params methods
    //
    // From bugs #46199 and #43367
    //
    using Uno;

    class MyTest {

            [Uno.Testing.Ignore, Uno.Testing.Test] public static void test_189() { Uno.Testing.Assert.AreEqual(0, Main(new string[0])); }
        public static int Main(string[] args)
            {
                    if (m (1, 2) != 0)
                            return 1;

                    MonoTest2 test = new MonoTest2 ();
                    if (test.method1 ("some message", "some string") != 0)
                            return 1;

                    return 0;
            }

            public static int m(int a, double b)
            {
                    return 1;
            }

            public static int m(int x0, params int[] xr)
            {
                    return 0;
            }
    }

    public class MonoTest
    {
            public virtual int method1 (string message, params object[] args)
            {
                    return 1;
            }

            public void testmethod ()
            {
                    method1 ("some message", "some string");
            }
    }

    public class MonoTest2 : MonoTest {

            public override int method1 (string message, params object[] args)
            {
                    return 0;
            }

            public void testmethod2 ()
            {
                    method1 ("some message ", "some string");
            }
    }
}
