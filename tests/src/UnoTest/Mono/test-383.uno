namespace Mono.test_383
{
    using Uno;
    
    public class X
    {
        public readonly int Data;
    
            public X testme (out int x)
        {
                    x = 1;
            return this;
            }
    
            public X ()
        {
                    int x, y;
    
                    y = this.testme (out x).Data;
                    Console.WriteLine("X is {0}", x);
            }
    
            [Uno.Testing.Test] public static void test_383() { Main(); }
        public static void Main()
        {
                    X x = new X ();
            }
    }
}
