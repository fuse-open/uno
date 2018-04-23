namespace Mono.test_878
{
    using Uno;
    
    public class Tests
    {
        [Uno.Testing.Test] public static void test_878() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            return 0;
        }
    
        void Test1 ()
        {
            int a;
            if (true) {
                a = 0;
            } else {
                a = 1;
            }
    
            Console.WriteLine (a);
        }
    
        void Test2 ()
        {
            int a;
            if (false) {
                a = 0;
            } else {
                a = 1;
            }
    
            Console.WriteLine (a);
        }
    }
}
