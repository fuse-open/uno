namespace Mono.test_747
{
    using Uno;
    
    class B : A
    {
        protected class BNested : ANested
        {
        }
    }
    
    class A : AA
    {
    }
    
    class AA
    {
        protected class ANested
        {
        }
    }
    
    class M
    {
        [Uno.Testing.Test] public static void test_747() { Main(); }
        public static void Main()
        {
        }
    }
}
