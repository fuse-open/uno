namespace Mono.test_278
{
    using Uno;
    
    struct Rect {
            int x;
    
            public int X { get { return x; } set { x = value; } }
    }
    
    class X {
            [Uno.Testing.Test] public static void test_278() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
                    Rect rect = new Rect ();
                    rect.X += 20;
                    Console.WriteLine ("Should be 20: " + rect.X);
                    return rect.X == 20 ? 0 : 1;
            }
    }
}
