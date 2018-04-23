namespace Mono.test_xml_024
{
    // Compiler options: -doc:xml-024.xml
    
    namespace Testing
    {
       /// <include/>
       public class Test
       {
        // warning
        /// <include file='a' />
        [Uno.Testing.Test] public static void test_xml_024() { Main(); }
        public static void Main()
        {
        }
    
        // warning
        /// <include path='/foo/bar' />
        public void Bar (int x)
        {
        }
    
        // warning
        /// <include file='there-is-no-such-file' path='/foo/bar' />
        public void Baz (int x)
        {
        }
       }
    }
}
