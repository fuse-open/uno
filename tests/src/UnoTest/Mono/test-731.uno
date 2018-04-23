namespace Mono.test_731
{
    using Uno;
    
    // Static array initializers test
    
    enum S
    {
        Foo = 5
    }
    
    class C
    {
        [Uno.Testing.Test] public static void test_731() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            S[] s = new S [] { S.Foo, S.Foo, S.Foo, S.Foo, S.Foo, S.Foo, S.Foo, S.Foo, S.Foo, S.Foo, S.Foo, S.Foo };
            Console.WriteLine (s [5]);
                
            return 0;
        }
    }
}
