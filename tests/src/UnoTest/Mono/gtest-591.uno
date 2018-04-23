namespace Mono.gtest_591
{
    // Compiler options: -r:gtest-591-lib.dll
    
    using Uno;
    
    public class E
    {
        public Uno.Collections.Dictionary<int, A.B<int>.C> F;
        [Uno.Testing.Test] public static void gtest_591() { Main(); }
        public static void Main()
        {
            var e = new E ();
            Console.WriteLine (e.F);
        }
    }
}
