namespace Mono.test_915
{
    class ClassMain
    {
        static bool test = true;
    
        [Uno.Testing.Test] public static void test_915() { Main(); }
        public static void Main()
        {
            if (true) {
                const int test = 0;
            }
            test = false;
        }
    }
}
