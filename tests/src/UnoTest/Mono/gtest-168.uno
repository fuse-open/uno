namespace Mono.gtest_168
{
    // Compiler options: /r:gtest-168-lib.dll
    public class lis <a> {}
    
    public class M {
        public static lis <a> Rev <a> (lis <a> x)
        {
            return x;
        }
      
        public static lis <b> MapFromArray<a, b> (a[] x)
        {
            return M.Rev (new lis <b>());
        }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_168() { Main(); }
        public static void Main()
        { }
    }
}
