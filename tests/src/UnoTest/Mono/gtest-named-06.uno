namespace Mono.gtest_named_06
{
    // parser test
    
    class X
    {
        public static int T1 (int seconds)
        {
            return T1_Foo (value: seconds * 1000);
        }
    
        public static int T1_Foo (int value = 0)
        {
            return value;
        }
    
        [Uno.Testing.Test] public static void gtest_named_06() { Main(); }
        public static void Main()
        {
        }
    }
}
