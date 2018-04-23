namespace Mono.test_xml_002
{
    // Compiler options: -doc:xml-002.xml
    using Uno;
    
    namespace Testing
    {
        /// <summary>
        /// comment on class
        /// </summary>
        public class Test
        {
            [Uno.Testing.Test] public static void test_xml_002() { Main(); }
        public static void Main()
            {
            }
        }
    
        /// <summary>
        /// Incorrect comment markup. See <see cref="T:Testing.Test" /> too.
        /// </incorrect>
        public class Test2
        {
        }
    
        /**
            <summary>
            another Java-style documentation style
            </summary>
        */
        public class Test3
        {
        }
    
            /// indentation level test <seealso
            ///    cref="T:Testing.Test" />.
    public class Test4
    {
    }
    
    }
}
