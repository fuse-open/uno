namespace Mono.test_xml_068
{
    // Compiler options: -doc:xml-068.xml
    
    class X
    {
        /// <summary>
        /// Test summary
        /// </summary>
        /// <see cref="#sometext"/>
        static void Test ()
        {
        }
    
        [Uno.Testing.Test] public static void test_xml_068() { Main(); }
        public static void Main()
        {
        }
    }
}
