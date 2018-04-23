namespace Mono.gtest_174
{
    // Compiler options: /r:gtest-174-lib.dll
    public class B<T> {
        public static B<T> _N_constant_object = new B<T> ();
    }
    
    class M {
        [Uno.Testing.Test] public static void gtest_174() { Main(); }
        public static void Main() {
            A<int> x = A<int>._N_constant_object;
            B<int> y = B<int>._N_constant_object;
        }
    }
}
