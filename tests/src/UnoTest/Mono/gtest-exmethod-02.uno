namespace Mono.gtest_exmethod_02
{
    // Compiler options: -r:gtest-exmethod-02-lib.dll -noconfig
    
    using Uno;
    
    public class M
    {
        [Uno.Testing.Test] public static void gtest_exmethod_02() { Main(); }
        public static void Main()
        {
            "foo".Test_1 ();
        }
    }
    
    namespace N
    {
        public class M
        {
            public static void Test2 ()
            {
                "foo".Test_1 ();
            }
        }
    }
}
