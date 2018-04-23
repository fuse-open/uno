namespace Mono.test_xml_023
{
    // Compiler options: -doc:xml-023.xml
       public class Test
       {
           public class A {}
    
           [Uno.Testing.Test] public static void test_xml_023() { Main(); }
        public static void Main()
           {
           }
    
           /// here is a documentation
           public static void Foo (A a, int x)
           {
           }
       }
}
