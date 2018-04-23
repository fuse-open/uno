namespace Mono.test_xml_058
{
    // Compiler options: -doc:xml-058.xml
    using Uno; 
    
    ///<summary>This file throws an error when compiled with XML documentation</summary>
    public class GenericClass <gT>
    {
        gT m_data; 
    
        ///<summary>This line caused bug #77183</summary>
        public GenericClass (gT Data)
        {
            m_data = Data; 
        }
    }
    
    class Foo
    {
        [Uno.Testing.Test] public static void test_xml_058() { Main(); }
        public static void Main() {}
    }
}
