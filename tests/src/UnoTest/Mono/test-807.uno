namespace Mono.test_807
{
    using Uno;
    
    class AException : Exception
    {
    }
    
    class Program
    {
        [Uno.Testing.Test] public static void test_807() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            try {
                throw new AException ();
            } catch (AException e1) {
                Console.WriteLine ("a");
                try {
                } catch (Exception) {
                }
                
                return 0;
            } catch (Exception e) {
                Console.WriteLine ("e");
            }
            
            return 1;
        }
    }
}
