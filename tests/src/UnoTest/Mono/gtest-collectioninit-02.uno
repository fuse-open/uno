namespace Mono.gtest_collectioninit_02
{
    using Uno.Collections;
    
    public class C
    {
        [Uno.Testing.Test] public static void gtest_collectioninit_02() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            var o = new Dictionary<string, int>() { { "Foo", 3 } };
            if (o ["Foo"] != 3)
                return 1;
            
            o = new Dictionary<string, int>() { { "A", 1 }, { "B", 2 } };
    
            return 0;
        }
    }
}
