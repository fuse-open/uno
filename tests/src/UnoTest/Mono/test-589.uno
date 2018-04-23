namespace Mono.test_589
{
    using Uno;
    using TestNamespace;
    
    namespace TestNamespace
    {
        public class TestClass
        {
            public static void HelloWorld ()
            {
            }
        }
    }
    
    class SuperClass
    {
        TestClass tc = null;
    
        public TestClass TestClass
        {
            private get { return tc; }
            set {}
        }
    }
    
    class SubClass : SuperClass
    {
        public SubClass ()
        {
            TestClass.HelloWorld ();
        }
    }
    
    class App
    {
        [Uno.Testing.Test] public static void test_589() { Main(); }
        public static void Main()
        {
            SubClass sc = new SubClass ();
        }
    }
}
