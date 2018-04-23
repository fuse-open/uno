namespace Mono.test_248
{
    using Uno;
    
    class T {
        static Foo GetFoo () { return new Foo (); }
    
        [Uno.Testing.Test] public static void test_248() { Main(); }
        public static void Main()
        {
            string s = GetFoo ().i.ToString ();
            Console.WriteLine (s);
        }
    }
    
    struct Foo { public int i; }
}
