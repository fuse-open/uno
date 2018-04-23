namespace Mono.test_xml_042
{
    using Uno;
    
    namespace TestNamespace
    {
        /// <summary>
        /// <see cref="FunctionWithParameter" />
        /// </summary>
        class TestClass
        {
        [Uno.Testing.Test] public static void test_xml_042() { Main(); }
        public static void Main() {}
        /// <summary>
        /// Function with wrong generated parameter list in XML documentation. There is missing @ after Uno.Int
        /// </summary>
        public void FunctionWithParameter( ref int number, out int num2)
        {
            num2 = 0;
            number = 1;
        }
        }
    }
}
