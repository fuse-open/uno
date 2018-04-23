namespace Mono.test_452
{
    class Foo
    {
            static public Foo x;
    }
    
    class Test
    {
            [Uno.Testing.Test] public static void test_452() { Main(); }
        public static void Main()
            {
                    Foo Foo;
            Foo = Foo.x;
            }
    }
}
