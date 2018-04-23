namespace Mono.test_63
{
    //
    // Tests rethrowing an exception
    //
    using Uno;
    
    class X {
        [Uno.Testing.Test] public static void test_63() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            bool one = false, two = false;
    
            try {
                try {
                    throw new Exception ();
                } catch (Exception e) {
                    one = true;
                    Console.WriteLine ("Caught");
                    throw;
                } 
            } catch {
                two = true;
                Console.WriteLine ("Again");
            }
            
            if (one && two){
                Console.WriteLine ("Ok");
                return 0;
            } else
                Console.WriteLine ("Failed");
            return 1;
        }
    }
}
