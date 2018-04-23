namespace Mono.test_609
{
    using Uno;
    
    class Test
    {    
        [Uno.Testing.Test] public static void test_609() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            if (!("aoeu" is String))
                return 1;
                
            if (!("aoeu" is Object))
                return 2;
                
            return 0;
        }
    }
}
