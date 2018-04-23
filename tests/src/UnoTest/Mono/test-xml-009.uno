namespace Mono.test_xml_009
{
    // Compiler options: -doc:xml-009.xml
    using Uno;
    
    namespace Testing
    {
        public class Test
        {
            [Uno.Testing.Test] public static void test_xml_009() { Main(); }
        public static void Main()
            {
                /// inside method - not allowed.
            }
        }
    
        public class Test2
        {
            /// no target
        }
    
        public class Test3
        {
        }
        /// no target case 2.
    }
}
