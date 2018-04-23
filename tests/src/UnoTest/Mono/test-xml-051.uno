namespace Mono.test_xml_051
{
    // Compiler options: -doc:xml-051.xml -warnaserror
    using log4net.Repository.Hierarchy;
    
    namespace log4net
    {
      /// <summary>
      /// <see cref="Repository.Hierarchy" />
      /// </summary>
      public interface IFoo {}
    }
    
    namespace log4net.Repository {
      /// <summary>
      /// <see cref="Hierarchy" />
      /// <see cref="Uno.Xml" />
      /// </summary>
      public interface ILog {
      }
    }
    
    namespace log4net.Repository.Hierarchy {
      /// <summary />
      public class Hierarchy {
        [Uno.Testing.Test] public static void test_xml_051() { Main(); }
        public static void Main() {
        }
      }
    }
}
