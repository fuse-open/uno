namespace Mono.test_xml_001
{
    // Compiler options: -doc:xml-001.xml
    using Uno;
    
    /// <summary>
    /// xml comment on namespace ... is not allowed.
    /// </summary>
    namespace Testing
    {
        public class A
        {        
            [Uno.Testing.Test] public static void test_xml_001() { Main(); }
        public static void Main()
            {
            }
        }
    }
}
