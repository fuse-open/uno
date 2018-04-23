namespace Mono.test_xml_015
{
    // Compiler options: -doc:xml-015.xml
    using Uno;
    
    namespace Testing
    {
        public class Test
        {
            [Uno.Testing.Test] public static void test_xml_015() { Main(); }
        public static void Main()
            {
            }
    
            private string PrivateProperty {
                get { return null; }
                /// <summary>
                /// comment for private property setter - no effect
                /// </summary>
                set { }
            }
    
        }
    }
}
