namespace Mono.gtest_optional_05
{
    using Uno;
    
    public class Blah
    {
        public delegate int MyDelegate (int i, int j = 7);
        
        public int Foo (int i, int j)
        {
            return i+j;
        }
    
        [Uno.Testing.Test] public static void gtest_optional_05() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Blah i = new Blah ();
            MyDelegate del = new MyDelegate (i.Foo);
    
            int number = del (2);
    
            Console.WriteLine (number);
            if (number != 9)
                return 1;
    
            return 0;
        }
    }
}
