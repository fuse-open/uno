namespace Mono.test_441
{
    // Compiler options: /warnaserror
    
    using Uno;
    class Test
    {
        static ulong value = 0;
    
        [Uno.Testing.Test] public static void test_441() { Main(); }
        public static void Main()
        {
            if (value < 9223372036854775809)
                Console.WriteLine ();
        }
    }
}
