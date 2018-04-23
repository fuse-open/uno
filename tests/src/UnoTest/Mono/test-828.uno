namespace Mono.test_828
{
    // Compiler options: -warnaserror
    
    public class C
    {
        public int v;
    }
    
    public struct S2
    {
        public C c;
        public int v;
    }
    
    public struct S
    {
        public S2 s2;
    }
    
    public class Test
    {
        [Uno.Testing.Test] public static void test_828() { Main(); }
        public static void Main()
        {
            S s;
            s.s2.v = 9;
        }
    }
}
