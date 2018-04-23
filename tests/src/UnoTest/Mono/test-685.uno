namespace Mono.test_685
{
    // Compiler options: -warnaserror
    
    // Checks redundant CS0642 warning
    
    public class C
    {
        [Uno.Testing.Test] public static void test_685() { Main(); }
        public static void Main()
        {
            int v;
            for (v = 1; v >= 0; v--) ;
            uint [] b = null;
            if (b != null)
                return;
        }
    }
}
