namespace Mono.gtest_initialize_06
{
    struct Point
    {
        public int X, Y;
    }
    
    class C
    {
        [Uno.Testing.Test] public static void gtest_initialize_06() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Point p;
            Foo (out p);
            
            if (p.X != 3)
                return 1;
            
            if (p.Y != 5)
                return 2;
            
            return 0;
        }
        
        static void Foo (out Point p)
        {
            p = new Point () { X = 3, Y = 5 };
        }
    }
}
