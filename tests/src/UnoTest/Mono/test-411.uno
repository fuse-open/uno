namespace Mono.test_411
{
    // Compiler options: -r:test-411-lib.dll
    
    namespace QtSamples
    {
        using Qt;
    
        public class QtClass: QtSupport
        {
            public QtClass()
            {
                mousePressEvent += new MousePressEvent( pressEvent );
            }
            
            public void pressEvent() { }
        }
    
    
        public class Testing
        {
            [Uno.Testing.Test] public static void test_411() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
                QtClass q = new QtClass();
    
                return 0;
            }
        }
    }
}
