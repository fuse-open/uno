namespace Mono.test_151
{
    using Uno;
    namespace A {
            public class Iface {
                    void bah() {}
            }
            class my {
                    A.Iface b;
                    void doit (Object A) {
                            b = (A.Iface)A;
                    }
                    [Uno.Testing.Test] public static void test_151() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main() {
                            return 0;
                    }
            }
    }
}
