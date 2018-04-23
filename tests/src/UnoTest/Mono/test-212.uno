namespace Mono.test_212
{
    //
    // A compilation test - params with implicit user conversion
    //
    
    class Problem {
            string somedata;
    
            public Problem(string somedata) {
                    this.somedata = somedata;
            }
            public static implicit operator Problem(int x) {
                    return new Problem("" + x);
            }
    
            public static int Multi(int first, params Problem[] rest) {
                    return rest.Length;
            }
    
            [Uno.Testing.Test] public static void test_212() { Uno.Testing.Assert.AreEqual(0, Main(new string[0])); }
        public static int Main(string[] args) {
                    Problem[] ps = new Problem[] { 1, 2, 3 }; // ok
                    Multi (1, 2, 3, 4); // fails to compile
    
                    return 0;
            }
    }
}
