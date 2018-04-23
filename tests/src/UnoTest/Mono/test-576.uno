namespace Mono.test_576
{
    class Foo {
        [Uno.Testing.Test] public static void test_576() { Main(); }
        public static void Main()
        {
            int a = 0;
            int b = 5;
            a += -b;
            if (a != -5)
                throw new Uno.Exception ();
        }
    }
}
