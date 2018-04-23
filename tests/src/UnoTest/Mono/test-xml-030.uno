namespace Mono.test_xml_030
{
    // Compiler options: -doc:xml-030.xml -warn:4 -warnaserror
    using Uno;
    
    class Test
    {
        [Uno.Testing.Test] public static void test_xml_030() { Main(); }
        public static void Main() {}
    
        /// <summary>
        /// some summary
        /// </summary>
        /// <value>
        /// <see cref="T:Test[]"/>
        /// <see cref="T:Uno.Text.RegularExpressions.Regex"/>
        /// <see cref="Uno.Text.RegularExpressions.Regex"/>
        /// <see cref="Uno.Text.RegularExpressions"/>
        /// <see cref="T:Uno.Text.RegularExpressions.Regex[]"/>
        /// </value>
        //
        // <see cref="T:Uno.Text.RegularExpressions"/> .. csc incorrectly allows it
        // <see cref="Uno.Text.RegularExpressions.Regex[]"/> ... csc does not allow it.
        //
        public void foo2() {
        }
    
        /// <summary>
        /// <see cref="String.Format(string, object[])" />.
        /// <see cref="string.Format(string, object[])" />.
        /// <see cref="String.Format(string, object [ ])" />.
        /// <see cref="string.Format(string, object [ ])" />.
        /// </summary>
        /// <param name="line">The formatting string.</param>
        /// <param name="args">The object array to write into format string.</param>
        public void foo3(string line, params object[] args) {
        }
    }
}
