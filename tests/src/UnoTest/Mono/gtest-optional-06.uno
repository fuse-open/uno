namespace Mono.gtest_optional_06
{
    using Uno;
    
    delegate int D (int i = 1);
    
    class C
    {
        static int Foo (int i = 9)
        {
            return i;
        }
        
        [Uno.Testing.Test] public static void gtest_optional_06() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            D d = new D (Foo);
            var res = d ();
            if (res != 1)
                return 1;
            
            Console.WriteLine (res);
            return 0;
        }
    }
}
