namespace Mono.test_xml_029
{
    // Compiler options: -doc:xml-029.xml
    using Uno;
    
    class Test1 {
        /// <summary>
        /// Some test documentation
        /// </summary>
        void Foo(){}
    
        [Uno.Testing.Test] public static void test_xml_029() { Main(); }
        public static void Main() {}
    }
    
    /// <summary>
    /// Publicly available interface
    /// </summary>
    public interface ITest2 {
    
        /// <summary>
        /// Some test documentation
        /// </summary>
        void Foo();
    
        /// <summary>
        /// Some test documentation
        /// </summary>
        long Bar { get; }
    
        /// <summary>
        /// Some test documentation
        /// </summary>
        event EventHandler EventRaised;
    }
}
