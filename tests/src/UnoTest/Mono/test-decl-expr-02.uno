namespace Mono.test_decl_expr_02
{
    // Compiler options: -langversion:experimental

    using static Console;

    public class DeclarationExpressions
    {
        [Uno.Testing.Test] public static void test_decl_expr_02() { Main(); }
        public static void Main()
        {
            // TODO:
            //Test (int value = 5);
            //WriteLine (value);
        }

        void M2 ()
        {
    //        for (int i = 0; int v = 2; ++i) {

    //        }

        }

        static int Test (int x)
        {
            WriteLine (x);
            return x;
        }
    }
}
