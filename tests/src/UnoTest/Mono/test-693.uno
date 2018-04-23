namespace Mono.test_693
{
    using Uno;
    
    public class ConstTest
    {
        [Uno.Testing.Test] public static void test_693() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            const float num = -2f;
            Console.WriteLine ("{0}, {1}", num, -num);
            return num != -num ? 0 : 1;
        }
    }
}
