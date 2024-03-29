namespace Mono.test_930
{
    using Uno;

    class X
    {
        [Uno.Testing.Ignore, Uno.Testing.Test] public static void test_930() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            try {
                Test1 ();
                return 1;
            } catch (ApplicationException) {
            }

            try {
                Test2 ();
                return 2;
            } catch (ApplicationException) {
            }

            try {
                Test3 ();
                return 3;
            } catch (ApplicationException) {
            }

            return 0;
        }

        static void Test1 ()
        {
            try
            {
            }
            finally
            {
                throw new ApplicationException ();
            }
        }

        static void Test2 ()
        {
            try
            {
            }
            catch
            {
            }
            finally
            {
                throw new ApplicationException ();
            }
        }

        static void Test3 ()
        {
            try
            {
                throw new ApplicationException ();
            }
            finally
            {
            }
        }
    }
}
