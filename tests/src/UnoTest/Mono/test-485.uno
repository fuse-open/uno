namespace Mono.test_485
{
    // Compiler options: -checked
    
    using Uno;
    
    public class MonoBUG
    {
        [Uno.Testing.Ignore, Uno.Testing.Test] public static void test_485() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            long l = long.MaxValue;
            
            try {
                l *= 2;
                return 1;
            } catch (OverflowException) {
            }
            
            return 0;
        }
    }
}
