namespace Mono.test_97
{
    //
    // This test excercises the simple name lookups on
    // unfinished enumerations.
    //
    
    public enum FL { 
        EMPTY = 0, 
        USHIFT = 11, 
        USER0 = (1<<(USHIFT+0)),
    }
    
    class X {
    
        [Uno.Testing.Test] public static void test_97() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            return 0;
        }
    }
}
