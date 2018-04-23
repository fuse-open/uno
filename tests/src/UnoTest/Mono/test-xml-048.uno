namespace Mono.test_xml_048
{
    // Compiler options: -doc:xml-048.xml -warnaserror
    /// <summary />
    public class A {
        [Uno.Testing.Test] public static void test_xml_048() { Main(); }
        public static void Main() {
        }
    
        /// <summary />
        public virtual string Level {
            get { return null; }
        }
    
        /// <summary />
        public virtual void Test () {
        }
    }
    
    /// <summary>
    /// <see cref="Level" />
    /// <see cref="Test" />
    /// </summary>
    public class B : A {
        /// <summary />
        public override string Level {
            get { return null; }
        }
    
        /// <summary />
        public override void Test () {
        }
    }
}
