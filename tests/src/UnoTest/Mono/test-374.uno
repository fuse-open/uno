namespace Mono.test_374
{
    internal class Test 
    {
        protected internal const int foo = 0;
    }
    internal class Rest
    {
        protected const int foo = Test.foo;
    
        [Uno.Testing.Test] public static void test_374() { Main(); }
        public static void Main() {}
    }
}
