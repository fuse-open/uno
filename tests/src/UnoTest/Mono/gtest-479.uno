namespace Mono.gtest_479
{
    using Uno;
    
    interface I<T>
    {
    }
    
    class A : I<int>
    {
    }
    
    class B : A
    {
    }
    
    class M
    {
        static void Test<T> (I<T> f)
        {
        }
        
        [Uno.Testing.Test] public static void gtest_479() { Main(); }
        public static void Main()
        {
            Test (new A ());
            Test (new B ());
        }
    }
}
