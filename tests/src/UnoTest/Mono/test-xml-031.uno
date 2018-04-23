namespace Mono.test_xml_031
{
    // Compiler options: -doc:xml-031.xml -warn:4 -warnaserror
    //// Some comment
    ////how about this line?
    using Uno;
    using Uno.IO;
    
    class Test
    {
        [Uno.Testing.Test] public static void test_xml_031() { Main(); }
        public static void Main()
        {
        }
    }
}
