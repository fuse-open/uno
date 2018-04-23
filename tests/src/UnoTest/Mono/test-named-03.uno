namespace Mono.test_named_03
{
    using Uno;
    
    class C
    {
        delegate int IntDelegate (int a);
        
        static int TestInt (int u)
        {
            return 29;
        }
        
        [Uno.Testing.Test] public static void test_named_03() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            var del = new IntDelegate (TestInt);
            del (a : 7);
            
            return 0;
        }
    }
}
