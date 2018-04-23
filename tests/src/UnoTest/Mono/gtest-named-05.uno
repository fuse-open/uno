namespace Mono.gtest_named_05
{
    class Test
    {
        [Uno.Testing.Test] public static void gtest_named_05() { Main(); }
        public static void Main()
        {
            string p;
            M (y: p = F (), x: p);
    
            int i;
            string p2;
            M2 (out i, c: p2 = F (), b : p2);
        }
    
        public static void M (string x, string y)
        {
        }
    
        static void M2 (out int a, string b, string c)
        {
            a = 2;
        }
    
        public static string F ()
        {
            return null;
        }
    }
}
