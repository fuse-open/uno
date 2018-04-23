namespace Mono.test_262
{
    namespace n1 {
        class Attribute {}
    }
    
    namespace n3 {
        using n1;
        using Uno;
        class A {
            void Attribute () {
            }
            void X ()
            {
                Attribute ();
            }
            [Uno.Testing.Test] public static void test_262() { Main(); }
        public static void Main() {
                new A ().X ();
            }
        }
    }
}
