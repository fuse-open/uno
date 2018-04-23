namespace Mono.test_131
{
    using Uno;
    
    public class SimpleAttribute : Attribute {
    
        string n;
        
        public SimpleAttribute (string name)
        {
            n = name;
        }
    }
    
    public class Blah {
    
        public enum Foo {
    
            A,
    
            [Simple ("second")]
            B,
    
            C
        }
    
        [Uno.Testing.Test] public static void test_131() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            //
            // We need a better test which does reflection to check if the
            // attributes have actually been applied etc.
            //
    
            return 0;
        }
    
    }
}
