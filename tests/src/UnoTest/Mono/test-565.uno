namespace Mono.test_565
{
    // Compiler options: -warn:4 -warnaserror
    
    class T
    {
        public new bool Equals (object obj)
        {
            return true;
        }
    
        [Uno.Testing.Test] public static void test_565() { Main(); }
        public static void Main()
        { }
    }
}
