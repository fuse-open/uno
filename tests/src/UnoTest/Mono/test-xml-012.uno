namespace Mono.test_xml_012
{
    // Compiler options: -doc:xml-012.xml
    using Uno;
    
    namespace Testing
    {
        public class Test
        {
            [Uno.Testing.Test] public static void test_xml_012() { Main(); }
        public static void Main()
            {
            }
    
            /// <summary>
            /// comment for private field
            /// </summary>
            private string PrivateField;
    
            /// <summary>
            /// incorrect markup comment for private field
            /// </incorrect>
            private string PrivateField2;
    
            /**
            <summary>
            Javadoc comment for private field
            </summary>
            */
            private string PrivateField3;
        }
    }
}
