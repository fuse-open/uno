namespace Mono.test_762
{
    using Uno;
    using N1;
    using N2;
    
    namespace N1.Derived {
        class Dummy {}
    }
    
    namespace N2.Derived {
        class Dummy {}
    }
    
    public class DerivedAttribute : Attribute {
    }
    
    [Derived ()]
    class T {
        [Uno.Testing.Test] public static void test_762() { Main(); }
        public static void Main() {}
    }
}
