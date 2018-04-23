namespace Mono.test_252
{
    // testcase from #58290
    
    delegate void Foo ();
    class A {
        public event Foo Bar;
    
        public static void m1 () { }
     
        [Uno.Testing.Test] public static void test_252() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            A a = new A();
            a.Bar += new Foo (m1);
            a.Bar -= new Foo (m1);
            return (a.Bar == null) ? 0 : 1;
        }
    }
}
