namespace Mono.gtest_156
{
    // Compiler options: -r:gtest-156-lib.dll
    
    namespace FLMID.Bugs.Marshal15
    {
        public class D : C
        {
            public D()
            {
                _layout = new X();
            }
            [Uno.Testing.Test] public static void gtest_156() { Main(new string[0]); }
        public static void Main(string[] args)
            {
                Console.WriteLine("OK");
            }
        }
    }
}
