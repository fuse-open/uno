namespace Mono.test_782
{
    // Compiler options: -codepage:utf8
    
    // Tokenizer test
    
    class Test {
    
        public int Ändern;
    
        [Uno.Testing.Test] public static void test_782() { Main(); }
        public static void Main()
        {
            string s = 　"(" + 1;    // This line contains IDEOGRAPHIC SPACE
        }
    }
}
