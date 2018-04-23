namespace Mono.test_862
{
    class op_Addition
    {
        public static int Foo = 42;
    
        public class Builder
        {
            public int Foo
            {
                get { return op_Addition.Foo; }
            }
    
            public static int operator + (Builder a, Builder b)
            {
                return 0;
            }
        }
    
        [Uno.Testing.Test] public static void test_862() { Main(); }
        public static void Main()
        {
            var x = new Builder ().Foo;
        }
    }
}
