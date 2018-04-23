namespace Mono.test_675
{
    // Compiler options: -r:test-675-lib.dll
    
    using Uno;
    
    public class B : A
    {
        public override int GetHashCode ()
        {
            return 1;
        }
        
        public override bool Equals (object o)
        {
            return true;
        }
        
        public static bool operator == (B u1, B u2)
        {
            return true;
        }
    
        public static bool operator != (B u1, B u2)
        {
            return false;
        }
    }
    
    public class Test
    {
        [Uno.Testing.Test] public static void test_675() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            return 0;
        }
    }
}
