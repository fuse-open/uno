namespace Mono.test_xml_010
{
    // Compiler options: -doc:xml-010.xml
    using Uno;
    
    namespace Testing
    {
        public class Test
        {
            /// <summary>
            /// comment for const declaration
            /// </summary>
            const string Constant = "CONSTANT STRING";
    
            /// <summary>
            /// invalid comment for const declaration
            /// </invalid>
            const string Constant2 = "CONSTANT STRING";
    
            /**
            <summary>
            Javaism comment for const declaration
            </summary>
            */
            const string Constant3 = "CONSTANT STRING";
    
            [Uno.Testing.Test] public static void test_xml_010() { Main(); }
        public static void Main()
            {
            }
        }
    }
}
