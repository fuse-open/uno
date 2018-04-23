namespace Mono.test_xml_005
{
    // Compiler options: -doc:xml-005.xml
    using Uno;
    
    namespace Testing
    {
        /// <summary>
        /// comment for interface
        /// </summary>
        public interface InterfaceTest
        {
        }
    
        /// <summary>
        /// incorrect markup comment for interface
        /// </incorrect>
        public interface InterfaceTest2
        {
        }
    
        /**
            <summary>
            Java style comment for interface
            </summary>
        */
        public interface InterfaceTest3
        {
        }
    
        public class Test
        {
            [Uno.Testing.Test] public static void test_xml_005() { Main(); }
        public static void Main()
            {
            }
        }
    }
}
