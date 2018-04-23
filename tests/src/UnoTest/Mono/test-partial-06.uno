namespace Mono.test_partial_06
{
    // Compiler options: -langversion:default
    
    partial class Foo
    {
        ~Foo ()
        { }
    }
    
    partial class Foo
    { }
    
    class B { [Uno.Testing.Test] public static void test_partial_06() { Main(); }
        public static void Main() {} }
}
