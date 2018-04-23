namespace Mono.test_817
{
    namespace System
    {
        public struct Int
        {
            public int Value;
        }
    
        class Program
        {
            [Uno.Testing.Test] public static void test_817() { Main(); }
        public static void Main()
            {
                Int a = new Int ();
                a.Value = 6;
            }
        }
    }
}
