namespace Mono.test_277
{
    // test for bug #56774
    
    class T {
        [Uno.Testing.Test] public static void test_277() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main() {
            return X (1);
        }
      
        static int X (byte x) {
            return 0;
        }
        static int X (short x) {
            return 1;
        }
    }
}
