namespace Mono.gtest_317
{
    // Bug #77963
    public class Foo<K>
    {
    }
    
    public class Bar<Q> : Foo<Bar<Q>.Baz>
    {
            public class Baz
            {
            }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_317() { Main(); }
        public static void Main()
        {
            Bar<int> bar = new Bar<int> ();
            Console.WriteLine (bar);
        }
    }
}
