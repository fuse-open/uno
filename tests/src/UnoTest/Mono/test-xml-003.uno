namespace Mono.test_xml_003
{
    // Compiler options: -doc:xml-003.xml
    using Uno;
    
    namespace Testing
    {
        public class Test
        {
            [Uno.Testing.Test] public static void test_xml_003() { Main(); }
        public static void Main()
            {
                /// here is an extraneous comment
            }
        }
    }
}
