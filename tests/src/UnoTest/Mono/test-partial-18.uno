namespace Mono.test_partial_18
{
    namespace N
    {
        partial class Foo
        {
        }
    }
    
    namespace N
    {
        using Uno;
    
        partial class Foo
        {
            public Foo ()
            {
                Console.Write ("Hello, world.\n");
            }
            [Uno.Testing.Test] public static void test_partial_18() { Main(); }
        public static void Main()
            {
            }
        }
    }
}
