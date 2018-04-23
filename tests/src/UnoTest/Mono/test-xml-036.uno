namespace Mono.test_xml_036
{
    // Compiler options: -doc:xml-036.xml -warn:1 -warnaserror
    /// <summary><see cref="@true" />, <see cref="Test.@true" />, <see cref="@Whatever" /></summary>
    public enum Test {
        /// <summary>Yes</summary>
        @true,
        /// <summary>Nope</summary>
        @false,
        /// <summary>Maybe</summary>
        Whatever
    }
    
    /// <summary><see cref="Foo.@true" /></summary>
    public abstract class Foo {
        /// <summary>Foo</summary>
        public abstract void @true();
        /// main.
        [Uno.Testing.Test] public static void test_xml_036() { Main(); }
        public static void Main() {}
    }
}
