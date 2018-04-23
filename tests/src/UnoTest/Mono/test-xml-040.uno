namespace Mono.test_xml_040
{
    // Compiler options: -doc:xml-040.xml -warnaserror -warn:4
    using Uno.Collections;
    
    /// <summary><see cref="IDictionary.this[object]" /></summary>
    public class Test {
        [Uno.Testing.Test] public static void test_xml_040() { Main(); }
        public static void Main() {
        }
    
        /// <summary> test indexer doc </summary>
        public string this [string name] {
            get { return null; }
        }
    }
}
