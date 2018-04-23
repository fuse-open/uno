namespace Mono.gtest_exmethod_17
{
    // Compiler options: -r:gtest-exmethod-17-lib.dll
    
    using Uno;
    using Testy;
    
    public static class MainClass
    {
        [Uno.Testing.Test] public static void gtest_exmethod_17() { Main(); }
        public static void Main()
        {
            Object o = new Object ();
            Console.WriteLine (o.MyFormat ("hello:{0}:{1}:", "there", "yak"));
        }
    }
}
