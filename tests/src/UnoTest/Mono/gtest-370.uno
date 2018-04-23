namespace Mono.gtest_370
{
    namespace N2
    {
        public class X<T>
        {
            private class A<T>
            {
                private class B<T>
                {
                    public class C<T>
                    {
                    }
                
                    internal C<T> foo;
                }
            }
        }
        
        class C
        {
            [Uno.Testing.Test] public static void gtest_370() { Main(); }
        public static void Main()
            {
            }
        }
    }
}
