namespace Mono.gtest_exmethod_34
{
    // Compiler options: -warnaserror
    
    public static class Program
    {
        static void Foo (this object o)
        {
        }
        
        [Uno.Testing.Test] public static void gtest_exmethod_34() { Main(); }
        public static void Main()
        {
            const object o = null;
            o.Foo ();
        }
    }
}
