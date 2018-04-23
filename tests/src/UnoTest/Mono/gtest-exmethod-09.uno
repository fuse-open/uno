namespace Mono.gtest_exmethod_09
{
    static class Test
    {
        public static void Foo<T> (this string p1)
        {
        }
    }
    
    class C
    {
        [Uno.Testing.Test] public static void gtest_exmethod_09() { Main(); }
        public static void Main()
        {
            //int x = Test.Foo<bool> ("bb");
            "a".Foo<bool> ();
        }
    }
}
