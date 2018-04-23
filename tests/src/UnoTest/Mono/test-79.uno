namespace Mono.test_79
{
    //
    // This test excercises the compiler being able to compute
    // correctly the return type in the presence of null (as null
    // will be implicitly convertible to anything
    //
    class X {
    
        [Uno.Testing.Test] public static void test_79() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            object o = null;
    
            string s = o == null ? "string" : null;
            string d = o == null ? null : "string";
    
            return 0;
        }
    }
}
