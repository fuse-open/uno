namespace Mono.test_160
{
    class B {
            public S s;
    }
    class S {
            public int a;
    }
    class T {
        static B foo;
    
            static int blah (object arg) {
                    B look = (B)arg;
            foo.s.a = 9;
            look.s.a = foo.s.a;
                    return look.s.a;
            }
    
            [Uno.Testing.Test] public static void test_160() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main() {
            // Compilation only test;
            return 0;
        }
    }
}
