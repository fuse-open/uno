namespace Mono.test_901
{
    using Uno;
    
    class X
    {
        [Uno.Testing.Test] public static void test_901() { Main(); }
        public static void Main()
        {
            int i;
            if (true) {
                i = 3;
            }
    
            Console.WriteLine (i);
    
            int i2;
            if (false) {
                throw new ApplicationException ();
            } else {
                i2 = 4;
            }
    
            Console.WriteLine (i2);
        }
    }
}
