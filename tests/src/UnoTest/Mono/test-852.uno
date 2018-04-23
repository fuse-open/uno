namespace Mono.test_852
{
    // Compiler options: -warnaserror
    
    public class Test
    {
    #pragma warning disable 1634
    #pragma warning suppress 56500
        [Uno.Testing.Test] public static void test_852() { Main(); }
        public static void Main()
        {
        }
    #pragma warning restore 1634
    }
}
