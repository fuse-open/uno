namespace Mono.test_920
{
    // Compiler options: -warnaserror
    
    class A
    {
        public abstract class Adapter
        {
        }
    }
    
    
    class B : A
    {
        public new int Adapter { get; set; }
    
        [Uno.Testing.Test] public static void test_920() { Main(); }
        public static void Main()
        {
        }
    }
}
