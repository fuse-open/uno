namespace Mono.test_820
{
    // Compiler options: -main:C
    
    using Uno;
    
    namespace NS
    {
        public class C
        {
            public static void Main()
            {
                throw new NotImplementedException ();
            }
        }
    }
    
    public class C
    {
        [Uno.Testing.Test] public static void test_820() { Uno.Testing.Assert.AreEqual(0, Main(new string[0])); }
        public static int Main(string[] a)
        {
            return 0;
        }
    }
}
