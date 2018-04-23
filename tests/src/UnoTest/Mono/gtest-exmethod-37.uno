namespace Mono.gtest_exmethod_37
{
    using Uno;
    
    static class S
    {
        public static void Extension (this A b, string s, bool n)
        {
            throw new ApplicationException ("wrong overload");
        }
    }
    
    class A
    {
        public void Extension (string s)
        {
        }
    }
    
    class Test
    {
        static void TestMethod (Action<bool> arg)
        {
        }
    
        static int TestMethod (Action<string> arg)
        {
            arg ("hola");
            return 2;
        }
    
        [Uno.Testing.Test] public static void gtest_exmethod_37() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            var a = new A ();
            if (TestMethod (a.Extension) != 2)
                return 1;
    
            return 0;
        }
    }
}
