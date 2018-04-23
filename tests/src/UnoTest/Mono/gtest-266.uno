namespace Mono.gtest_266
{
    class Test<T>
    {
            int priv;
            private sealed class Inner<U>
            {
                    Test<U> test;
                    void Foo ()
                    {
                            test.priv = 0;
                    }
            }
    }
    
    class Test { [Uno.Testing.Test] public static void gtest_266() { Main(); }
        public static void Main() { } }
}
