namespace Mono.test_163
{
    using Uno;
    
    public class Blah {
    
        static int Foo (string s)
        {
            return 2;
        }
    
        static int Foo (object o)
        {
            return 1;
        }
    
        [Uno.Testing.Test] public static void test_163() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            int i = Foo (null);
    
            if (i == 1) {
                Console.WriteLine ("Wrong method ");
                return 1;
            }
            
            return 0;
        }
    
    }
}
