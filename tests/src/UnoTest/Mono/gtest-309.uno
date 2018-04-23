namespace Mono.gtest_309
{
    class Test<A,B>
    {
        public void Foo<V,W> (Test<A,W> x, Test<V,B> y)
        { }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_309() { Main(); }
        public static void Main()
        {
            Test<float,int> test = new Test<float,int> ();
            test.Foo (test, test);
        }
    }
}
