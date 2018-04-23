namespace Mono.test_332
{
    /* foo */
    #define FOO
    
    /* bar */ // bar again
    #define BAR
    
    public class C
    {
        [Uno.Testing.Test] public static void test_332() { Main(); }
        public static void Main() {}
    }
}
