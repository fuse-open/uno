namespace Mono.test_678
{
    using Uno;

    public class NaNTest
    {
        public NaNTest (double width, double height)
        {
            if (width < 0 || height < 0)
                throw new ArgumentException ("fails");

            if (width <= 0 || height <= 0)
                throw new ArgumentException ("fails 2");

            if (width > 0 || height > 0)
                throw new ArgumentException ("fails 3");

            if (width >= 0 || height >= 0)
                throw new ArgumentException ("fails 4");
        }

        [Uno.Testing.Test] public static void test_678() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            if (Double.NaN < 0 || Double.NaN < 0)
                throw new ArgumentException ("passes");

            new NaNTest (Double.NaN, Double.NaN);

            return 0;
        }
    }
}
