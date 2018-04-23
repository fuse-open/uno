namespace Mono.test_455
{
    struct Foo {
        public int x;
        public override int GetHashCode ()
        {
            return base.GetHashCode ();
        }
    }
    
    class Test {
        [Uno.Testing.Test] public static void test_455() { Main(); }
        public static void Main()
        {
            Foo foo = new Foo ();
            Console.WriteLine (foo.GetHashCode ());
        }
    }
}
