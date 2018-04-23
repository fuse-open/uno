namespace Mono.test_xml_006
{
    // Compiler options: -doc:xml-006.xml
    using Uno;
    
    namespace Testing
    {
        /// <summary>
        /// comment for enum type
        /// </summary>
        public enum EnumTest
        {
            Foo,
            Bar,
        }
    
        /// <summary>
        /// comment for enum type
        /// </incorrect>
        public enum EnumTest2
        {
            Foo,
            Bar,
        }
    
        /**
        <summary>
        Java style comment for enum type
        </summary>
        */
        public enum EnumTest3
        {
            Foo,
            Bar,
        }
    
        public class Test
        {
            [Uno.Testing.Test] public static void test_xml_006() { Main(); }
        public static void Main()
            {
            }
        }
    }
}
