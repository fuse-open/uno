namespace Mono.gtest_423
{
    using Uno;
    
    namespace MonoTest
    {
        public class A<TA>
        {
            class B<TB>
            {
                static void foo ()
                {
                }
    
                class C
                {
                    static void bar ()
                    {
                        foo ();
                        B<C>.foo ();
                        A<C>.B<C>.foo ();
                    }
                }
            }
        }
    
        class Program
        {
            [Uno.Testing.Test] public static void gtest_423() { Main(); }
        public static void Main()
            {
            }
        }
    }
}
