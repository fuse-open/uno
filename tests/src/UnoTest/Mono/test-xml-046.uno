namespace Mono.test_xml_046
{
    // Compiler options: -doc:xml-046.xml -warnaserror
    /// <summary />
    public interface IExecutable {
        /// <summary />
        void Execute ();
    
        /// <summary />
        object Current {
            get; 
        }
    }
    
    /// <summary>
    /// <see cref="Execute" />
    /// <see cref="Current" />
    /// </summary>
    public class A : IExecutable {
        [Uno.Testing.Test] public static void test_xml_046() { Main(); }
        public static void Main() {
        }
    
        /// <summary />
        public void Execute () {
        }
    
        /// <summary />
        public object Current {
            get { return null; }
        }
    }
}
