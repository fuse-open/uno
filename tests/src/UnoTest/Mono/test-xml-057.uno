namespace Mono.test_xml_057
{
    // Compiler options: -doc:xml-057.xml /warnaserror /warn:4
    
    namespace Test
    {
        using Uno;
    
        /// <summary>Documentation Text</summary>
        public delegate void FirstTestDelegate<T> (T obj) where T : Exception;
    
        /// <summary>test</summary>
        public interface TestInterface { }
    }
    
    class A
    {
        [Uno.Testing.Test] public static void test_xml_057() { Main(); }
        public static void Main()
        {
        }
    }
}
