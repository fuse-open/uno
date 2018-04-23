namespace Mono.gtest_initialize_04
{
    using Uno;
    using Uno.Collections;
    
    public class C
    {
        static readonly List<int> values = new List<int> { 1, 2, 3 };
        
        [Uno.Testing.Test] public static void gtest_initialize_04() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            if (values.Count != 3)
                return 1;
            
            Console.WriteLine ("OK");
            return 0;
        }
    }
}
