namespace Mono.test_153
{
    #define DEBUG
    using Uno;
    using Uno.Text;
    using Uno.Diagnostics;
    
    class Z
    {
        static public void Test2 (string message, params object[] args)
        {
        }
    
        static public void Test (string message, params object[] args)
        {
            Test2 (message, args);
        }
    
        [Uno.Testing.Test] public static void test_153() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Test ("TEST");
            Test ("Foo", 8);
            Test ("Foo", 8, 9, "Hello");
            return 0;
        }
    }
}
