namespace Mono.test_xml_008
{
    // Compiler options: -doc:xml-008.xml
    using Uno;
    
    namespace Testing
    {
        /// comment without markup on class - it is allowed
        public class Test
        {
            [Uno.Testing.Test] public static void test_xml_008() { Main(); }
        public static void Main()
            {
            }
        }
    
        /// <6roken> broken markup
        public class Test2
        {
        }
    
        /// <dont-forget-close-tag>
        public class Test3
        {
        }
    }
}
