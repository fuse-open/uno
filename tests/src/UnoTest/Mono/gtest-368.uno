namespace Mono.gtest_368
{
    class C<T> 
    {
        class D {}
        
        C(D d) {}
        public C() {}
    }
    
    
    class M
    {
        [Uno.Testing.Test] public static void gtest_368() { Main(); }
        public static void Main()
        {
        }
    }
}
