namespace Mono.test_758
{
    // Compiler options: -warn:4 -warnaserror
    
    public class C
    {
        public int Finalize;
        
        [Uno.Testing.Test] public static void test_758() { Main(); }
        public static void Main()
        {
        }
    }
    
    public class D : C
    {
        ~D ()
        {
        }
    }
}
