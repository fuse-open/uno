namespace Mono.test_559
{
    // Compiler options: -r:test-559-lib.dll
    
    class C : B
    {
        public override event TestDelegate TestEvent;
        
        [Uno.Testing.Test] public static void test_559() { Main(); }
        public static void Main()
        {
        }
    }
}
