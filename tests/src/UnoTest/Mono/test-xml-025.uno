namespace Mono.test_xml_025
{
    // Compiler options: -doc:xml-025.xml dlls/test-xml-025-relative.cs
    
    namespace Testing
    {
       /// <include file='test-xml-025.inc' path='/foo' />
       public class Test
       {
        [Uno.Testing.Test] public static void test_xml_025() { Main(); }
        public static void Main()
        {
        }
    
        /// <include file='test-xml-025.inc' path='/root'/>
        public string S1;
    
        /// <include file='test-xml-025.inc' path='/root/child'/>
        public string S2;
    
        /// <include file='test-xml-025.inc' path='/root/@attr'/>
        public string S3;
       }
    }
}
