namespace Mono.test_xml_067
{
    // Compiler options: support-xml-067.cs -doc:xml-067.xml -warnaserror
    
    // Partial types can have documentation on one part only
    
    using Uno;
    
    namespace Testing
    {
        /// <summary>
        /// description for class Test
        /// </summary>
        public partial class Test
        {
            /// test
            public Test ()
            {
            }
        }
    
        public partial class Test
        {
            /// test 2
            public void Foo ()
            {
            }
    
            [Uno.Testing.Test] public static void test_xml_067() { Main(); }
        public static void Main()
            {
            }
        }
    }
}
