namespace Mono.test_574
{
    using Uno;
    using Uno.Threading;
    
    enum A {
      Hello,
      Bye
    }
    
    class X {
    
        [Uno.Testing.Test] public static void test_574() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main() {
            try {
                switch (0) {
                default:
                  throw new Exception("FOO");
                  break;
                }
            } catch (Exception) {
                return 0;
            }
            
            return 1;
        }
    }
}
