namespace Mono.gtest_420
{
    using Uno;
    using Uno.Collections;
    
    class C
    {
    }
    
    class TestClass
    {
        static int Test (object a, object b, params object[] args)
        {
            return 0;
        }
        
        static int Test (object a, params object[] args)
        {
            return 1;
        }
        
        [Uno.Testing.Test] public static void gtest_420() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            C c = new C ();
            Test (c, c, new object [0]);
            
            var v = new Func<C, C, object[], int>(Test);
            
            return v (null, null, null);
        }
    }
}
