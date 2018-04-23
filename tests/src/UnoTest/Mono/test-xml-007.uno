namespace Mono.test_xml_007
{
    // Compiler options: -doc:xml-007.xml
    using Uno;
    
    namespace Testing
    {
        /// <summary>
        /// comment for delegate type
        /// </summary>
        public delegate void MyDelegate (object o, EventArgs e);
    
        /// <summary>
        /// comment for delegate type
        /// </incorrect>
        public delegate void MyDelegate2 (object o, EventArgs e);
    
        /**
        <summary>
        Javadoc comment for delegate type
        </summary>
        */
        public delegate void MyDelegate3 (object o, EventArgs e);
    
        public class Test
        {
            [Uno.Testing.Test] public static void test_xml_007() { Main(); }
        public static void Main()
            {
            }
        }
    }
}
