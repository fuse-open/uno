namespace Mono.gtest_477
{
    using Uno;
    
    class B<T> : A<T>
    {
        protected class BNested : ANested
        {
        }
    }
    
    class A<T> : AA<T>
    {
    }
    
    class AA<T>
    {
        protected class ANested
        {
        }
    }
    
    class M
    {
        [Uno.Testing.Test] public static void gtest_477() { Main(); }
        public static void Main()
        {
        }
    }
}
