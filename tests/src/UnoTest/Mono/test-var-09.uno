namespace Mono.test_var_09
{
    using Uno;
    
    class A
    {
        [Uno.Testing.Test] public static void test_var_09() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            var list = new A ();
            var a = list as object;
            object o = a;
            return 0;
        }
    }
}
