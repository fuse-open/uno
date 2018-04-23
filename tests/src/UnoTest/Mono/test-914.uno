namespace Mono.test_914
{
    // CS0135: `test' conflicts with a declaration in a child block
    // Line: 11
    
    class ClassMain {
            static bool test = true;
        
            [Uno.Testing.Test] public static void test_914() { Main(); }
        public static void Main() {
                    if (true) {
                            bool test = false;
                    }
                    test = false;
            }
    }
}
