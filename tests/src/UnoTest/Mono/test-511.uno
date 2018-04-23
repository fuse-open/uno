namespace Mono.test_511
{
    using Uno;
    
    public class Foo {
        [Uno.Testing.Test] public static void test_511() { Main(new string[0]); }
        public static void Main(string[] args)
        {
            try {
                f ();
            }
            catch {}
        }
    
        static void f ()
        {
            throw new Exception ();
            string hi;
            try { }
            finally {
                Console.WriteLine ("hi = {0}", hi);
            }
        }
    }
}
