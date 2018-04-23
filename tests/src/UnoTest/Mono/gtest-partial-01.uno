namespace Mono.gtest_partial_01
{
    using Uno;
    
    class B<U>
    {
    }
    
    partial class C<T> : B<T>
    {
        T t1;
    }
    
    partial class C<T> : B<T>
    {
        T t2;
    }
    
    class Test
    {
        [Uno.Testing.Test] public static void gtest_partial_01() { Main(); }
        public static void Main()
        {
        }
    }
}
