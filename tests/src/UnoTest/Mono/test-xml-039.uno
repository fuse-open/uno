namespace Mono.test_xml_039
{
    // Compiler options: -doc:xml-039.xml -warnaserror
    using Uno;
    
    /// <summary>
    /// <see cref="ITest.Start" />
    /// <see cref="ITest.Foo" />
    /// </summary>
    public interface ITest {
            /// <summary>whatever</summary>
            event EventHandler Start;
        /// <summary>hogehoge</summary>
        int Foo { get; }
    }
    
    class Test
    {
        [Uno.Testing.Test] public static void test_xml_039() { Main(); }
        public static void Main() {}
    }
}
