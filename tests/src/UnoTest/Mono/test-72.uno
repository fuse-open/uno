namespace Mono.test_72
{
    //
    // Compile test for referencing types on nested types
    //
    
    using Uno;
    
    public class outer {
            public class inner {
                    public void meth(Object o) {
                            inner inst = (inner)o;
                    }
            }
        
        [Uno.Testing.Test] public static void test_72() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            // We only test that this compiles.
            
            return 0;
        }
      }
}
