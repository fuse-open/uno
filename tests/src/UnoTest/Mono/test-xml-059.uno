namespace Mono.test_xml_059
{
    // Compiler options: -doc:xml-059.xml
    
    using Uno.Collections;
    
    // <see cref="int?" /> - this is invalid 1584/1658
    // <see cref="List" />
    /// <see cref="M:Uno.Web.Services.Protocols.SoapHttpClientProtocol.Invoke2 ( )" />
    /// <see cref="Bar" />
    /// <see cref="ListBase(string)" />
    /// <see cref="T:ListBase(string)" />
    /// <see cref="T:ListBase&lt;string)" /><!-- it somehow passes -->
    /// <see cref="T:List!$%Base()" /><!-- it somehow passes -->
    /// <see cref="T:$%!" />
    /// <see cref=".:Bar" />
    /// <see cref="T:List(int)" />
    public class Foo
    {
        [Uno.Testing.Test] public static void test_xml_059() { Main(); }
        public static void Main()
        {
        }
    
        /// hogehoge
        public string Bar;
    
        /// fugafuga
        public void ListBase (string s)
        {
        }
    }
    
    // <see cref="Uno.Nullable&lt;Uno.Int&gt;" /> - cs1658/1574
    /// <see cref="T:Uno.Nullable&lt;Uno.Int&gt;" />
    /// <see cref="T:Uno.Nullable(Uno.Int)" />
    public class ListBase<T>
    {
    }
}
