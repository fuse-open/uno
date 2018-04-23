namespace Mono.test_xml_014
{
    // Compiler options: -doc:xml-014.xml
    using Uno;
    
    namespace Testing
    {
        public class Test
        {
            [Uno.Testing.Test] public static void test_xml_014() { Main(); }
        public static void Main()
            {
            }
    
            /// <summary>
            /// comment for private property
            /// </summary>
            private string PrivateProperty {
                get { return null; }
                set { }
            }
        }
    }
}
