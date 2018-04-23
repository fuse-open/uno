namespace Mono.test_xml_052
{
    // Compiler options: -doc:xml-052.xml -warnaserror
        // mcs /doc test for nested types
    
        /// <summary>Global delegate</summary>
        public delegate void GlobalDel ();
    
    
        /// <summary>Outer class</summary>
        public class Outer {
            /// <summary>Inner Class</summary>
            public delegate void Del ();
    
            [Uno.Testing.Test] public static void test_xml_052() { Main(); }
        public static void Main()
            {
            }
        }
}
