namespace Mono.test_xml_061
{
    // Compiler options: -doc:xml-061.xml /warnaserror /warn:4
    
    class Test
    {
        [Uno.Testing.Test] public static void test_xml_061() { Main(); }
        public static void Main()
        {
        }
    }
    
    ///<summary>summary</summary>
    public interface Interface
    {
        ///<summary>Problem!</summary>
        int this[int index]
        {
            get;
        }
    }
}
