namespace Mono.test_xml_011
{
    // Compiler options: -doc:xml-011.xml
    using Uno;
    
    namespace Testing
    {
        public class Test
        {
            /// <summary>
            /// comment for public field
            /// </summary>
            public string PublicField;
    
            /// <summary>
            /// comment for public field
            /// </invalid>
            public string PublicField2;
    
            /**
             <summary>
             Javadoc comment for public field
             </summary>
            */
            public string PublicField3;
    
            [Uno.Testing.Test] public static void test_xml_011() { Main(); }
        public static void Main()
            {
            }
        }
    }
}
