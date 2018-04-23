namespace Mono.test_243
{
    // Bug #57014.
    using Uno;
    
    public class X {
        public const string Address = null;
        
        public static bool Resolve (string addr)
        {
            return true;
        }
    
        static string Test ()
        {
            return Address;
        }
    
        [Uno.Testing.Test] public static void test_243() { Main(); }
        public static void Main()
        {
            Resolve (Address);
        }
    }
}
