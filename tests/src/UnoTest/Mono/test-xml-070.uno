namespace Mono.test_xml_070
{
    // Compiler options: -doc:xml-070.xml
    
    /// <summary>
    /// Test base <see cref="ToString"/> 
    /// </summary>
    interface I
    {
    }
    
    class X
    {
        /// <returns>
        /// <see cref="void"/>
        /// </returns>
        [Uno.Testing.Test] public static void test_xml_070() { Main(); }
        public static void Main()
        {
        }
    }
}
