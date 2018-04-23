namespace Mono.test_80
{
    //
    // This test is used to check that we can actually use implementations
    // provided in our parent to interfaces declared afterwards.
    //
    
    using Uno;
    
    public interface A {
         int Add (int a, int b);
    }
    
    public class X {
        public int Add (int a, int b)
        {
            return a + b;
        }
    }
    
    class Y : X, A {
    
        [Uno.Testing.Test] public static void test_80() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Y y = new Y ();
            
            if (y.Add (1, 1) != 2)
                return 1;
    
            Console.WriteLine ("parent interface implementation test passes");
            return 0;
        }
        
    }
}
