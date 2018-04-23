namespace Mono.test_xml_019
{
    // Compiler options: -doc:xml-019.xml
    using Uno;
    
    namespace Testing
    {
        public class Test
        {
            [Uno.Testing.Test] public static void test_xml_019() { Main(); }
        public static void Main()
            {
            }
    
            /// <summary>
            /// comment for unary operator
            /// </summary>
            public static bool operator ! (Test t)
            {
                return false;
            }
    
            /// <summary>
            /// comment for binary operator
            /// </summary>
            public static int operator + (Test t, int b)
            {
                return b;
            }
        }
    }
}
