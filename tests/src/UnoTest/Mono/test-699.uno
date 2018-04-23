namespace Mono.test_699
{
    // Compiler options: -r:test-699-lib.dll
    
    public class D : C
    {
        string _message = "";
    
        public D (string msg)
        {
            _message = msg;
        }
    
        public string message
        {
            get { return _message; }
        }
    
        [Uno.Testing.Test] public static void test_699() { Main(); }
        public static void Main()
        {
        }
    }
}
