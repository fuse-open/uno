namespace Mono.test_231
{
    class T {
        static int ret_code = 0;
        
        [Uno.Testing.Test] public static void test_231() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            try {
                T t = null;
                t.Foo ();
            } catch {
                return ret_code;
            }
            ret_code = 1;
            return ret_code;
        }
        
        void Foo () {
            if (this == null) {
                Console.WriteLine ("This isnt anything!?!?");
                ret_code = 1;
            }
        }
    }
}
