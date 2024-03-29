namespace Mono.test_xml_026
{
    // Compiler options: -doc:xml-026.xml
    namespace Testing
    {
        /// test class
        public class Test
        {
            [Uno.Testing.Test] public static void test_xml_026() { Main(); }
        public static void Main()
            {
            }

            /// <param>anonymous</param>
            public void Foo (int i) {}

            /// <param name='i'>correct</param>
            /// <param name='i'>duplicate</param>
            public void Bar (int i) {}

            /// <param name='mismatch'>mismatch</param>
            public void Baz (int i) {}

            /// <param name='arr'>varargs</param>
            public void Var (params int [] arr) {}
        }
    }
}
