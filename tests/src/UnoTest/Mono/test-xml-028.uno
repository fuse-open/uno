namespace Mono.test_xml_028
{
    // Compiler options: -doc:xml-028.xml
    using Uno;
    
    /// <summary>
    /// Partial comment #2
    public partial class Test
    {
        string Bar;
    
        [Uno.Testing.Test] public static void test_xml_028() { Main(); }
        public static void Main() {}
    
        /// <summary>
        /// Partial inner class!
        internal partial class Inner
        {
            public string Hoge;
        }
    }
    
    /// Partial comment #1
    /// </summary>
    public partial class Test
    {
        public string Foo;
    
        /// ... is still available.
        /// </summary>
        internal partial class Inner
        {
            string Fuga;
        }
    }
}
