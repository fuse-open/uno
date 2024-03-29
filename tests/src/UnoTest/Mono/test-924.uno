namespace Mono.test_924
{
    using Uno;

    class MainClass
    {
        [Uno.Testing.Test] public static void test_924() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            DataFrame df1 = new DataFrame ();
            DataFrame df2 = new DataFrame ();

            if (df1 != null)
            {
                return 1;
            }

            return 0;
        }

        public class DataFrame
        {
            public static bool operator ==(DataFrame df1, DataFrame df2)
            {
                return df1 is DataFrame;
            }

            public static bool operator !=(DataFrame df1, DataFrame df2)
            {
                return !(df1 == df2);
            }
        }
    }
}
