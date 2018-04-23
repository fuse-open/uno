namespace Mono.test_619
{
    class X
    {
        [Uno.Testing.Test] public static void test_619() { Main(); }
        public static void Main()
        {
            while (true) {
                if (true)
                    break;
    
                continue;
            }
        }
    }
}
