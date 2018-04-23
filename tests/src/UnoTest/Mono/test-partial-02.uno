namespace Mono.test_partial_02
{
    // Compiler options: -langversion:default
    
    namespace Test1
    {
        public class Base
        { }
    
        public partial class Foo : Base
        { }
    
        public partial class Foo : Base
        { }
    }
    
    namespace Test2
    {
        public interface Base
        { }
    
        public partial class Foo : Base
        { }
    
        public partial class Foo : Base
        { }
    }
    
    public partial class ReflectedType { }
    partial class ReflectedType { }
    
    partial class D { }
    public partial class D { }
    partial class D { }
    
    class X
    {
        [Uno.Testing.Test] public static void test_partial_02() { Main(); }
        public static void Main()
        { }
    }
}
