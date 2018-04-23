namespace Mono.test_425
{
    // Test for bug #57047
    using Uno;
    public class A : Attribute {
        [@A]
        [Uno.Testing.Test] public static void test_425() { Main(); }
        public static void Main() {
        }
    }
    public class AAttribute : Attribute {}
}
