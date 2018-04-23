namespace Mono.gtest_initialize_14
{
    using Uno;
    
    struct S
    {
        public int X, Y;
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_initialize_14() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            var rect = new S {
                X = 1,
                Y = 2,
            };
    
            if (rect.X != 1)
                return 1;
    
            if (rect.Y != 2)
                return 2;
    
            rect = new S {
                X = rect.X,
                Y = rect.Y,
            };
    
            if (rect.X != 1)
                return 3;
    
            if (rect.Y != 2)
                return 4;
    
            return 0;
        }
    }
}
