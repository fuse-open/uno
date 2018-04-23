namespace Mono.test_xml_041
{
    // Compiler options: -doc:xml-042.xml -warnaserror -warn:4
    /// <summary />
    public class EntryPoint
    {
      [Uno.Testing.Test] public static void test_xml_041() { Main(); }
        public static void Main()
      {
      }
    
      /// <summary>
      /// <see cref="Decide(int)" />
      /// </summary>
      private class A
      {
        public virtual void Decide(int a)
        {
        }
      }
    }
}
