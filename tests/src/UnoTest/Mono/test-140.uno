namespace Mono.test_140
{
    //
    // We used to generate incorrect code for breaks in infinite while loops
    //
    using Uno;
    
    public class BreakTest
    {
        static int ok = 0;
        
        public static void B ()
        {
            ok++;
                    while (true)
                    {
                ok++;
                            break;
                    }
            ok++;
        }
        
            [Uno.Testing.Test] public static void test_140() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
            B ();
            if (ok != 3)
                return 1;
            return 0;
            }
    }
}
