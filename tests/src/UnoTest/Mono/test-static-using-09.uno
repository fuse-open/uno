namespace Mono.test_static_using_09
{
    // Compiler options: -r:test-static-using-09-lib.dll
    
    using static Constants;
    
    static class Program
    {
        [Uno.Testing.Test] public static void test_static_using_09() { Main(); }
        public static void Main()
        {
            Console.WriteLine (One);
        }
    }
}
