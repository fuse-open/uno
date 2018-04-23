namespace Mono.test_379
{
    using Uno;
    
    public class DeadCode {
    
        [Uno.Testing.Ignore, Uno.Testing.Test] public static void test_379() { Main(); }
        public static void Main()
        {
            SomeFunc ("...");
        }
    
        static public string SomeFunc (string str)
        {
            return str;
            int i = 0, pos = 0;
        }
    
    }
}
