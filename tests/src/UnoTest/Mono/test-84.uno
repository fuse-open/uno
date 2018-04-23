namespace Mono.test_84
{
    //
    // This test shows how a variable can be created with the 
    // same name as the class, and then the class referenced again
    // 
    // This was a bug exposed by Digger, as we incorrectly tried to
    // do some work ahead of time during the resolution process
    // (ie, we created LocalVariableReferences for the int variable `Ghost', 
    // which stopped `Ghost' from being useful as a type afterwards
    //
    
    class Ghost {
    
        [Uno.Testing.Test] public static void test_84() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            int Ghost = 0;
            
            if (true){
                Ghost g = null;
            }
            return 0;
        }
    }
}
