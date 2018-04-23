namespace Mono.gtest_initialize_09
{
    class Test
    {
        struct Foo { public int[] Data; }
    
        [Uno.Testing.Test] public static void gtest_initialize_09() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            int[] res = new Foo () { Data = new int[] { 1, 2, 3 } }.Data;
            if (res.Length != 3)
                return 1;
    
            return 0;
        }
    }
}
