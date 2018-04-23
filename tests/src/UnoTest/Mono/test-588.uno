namespace Mono.test_588
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
    
        TestClass TestClass
        {
            get { return tc; }
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
        [Uno.Testing.Test] public static void test_588() { Main(); }
        public static void Main()
        {
            SubClass sc = new SubClass ();
        }
    }
}
