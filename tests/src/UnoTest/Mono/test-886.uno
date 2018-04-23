namespace Mono.test_886
{
    public class A
    {
        public static A Get ()
        {
            return null;
        }
    }
    
    public class Test
    {
        void M ()
        {
            A A = A.Get ();
        }
    
        [Uno.Testing.Test] public static void test_886() { Main(); }
        public static void Main()
        {
            var t = new Test ();
            t.M ();
        }
    }
}
