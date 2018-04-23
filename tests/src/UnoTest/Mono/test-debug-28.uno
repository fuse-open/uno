namespace Mono.test_debug_28
{
    public class StatementsAsBlocks
    {
        static void ForEach (string[] args)
        {
            foreach (var v in args)
                ;
            foreach (var v in args)
                ;
        }
        
        [Uno.Testing.Test] public static void test_debug_28() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            return 0;
        }
    }
}
