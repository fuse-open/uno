namespace Mono.gtest_initialize_10
{
    using Uno;
    
    class Foo
    {
        public int P { get; set; }
    }
    
    class Y
    {
        [Uno.Testing.Test] public static void gtest_initialize_10() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Foo foo = new Foo ();
            foo.P = 1;
    
            if (!Do (foo))
                return 1;
    
            Console.WriteLine ("OK");
            return 0;
        }
    
        static bool Do (Foo f)
        {
            f = new Foo () {
                P = f.P
            };
    
            if (f.P != 1)
                return false;
    
            Foo f2 = new Foo ();
            f2.P = 9;
            f2 = new Foo () {
                P = f2.P
            };
    
            if (f2.P != 9)
                return false;
    
            return true;
        }
    }
}
