namespace Mono.test_732
{
    class C
    {
        public static explicit operator int (C c)
        {
            return 1;
        }
        
        public static int op_Implicit (C c, bool b)
        {
            return -1;
        }
    
        [Uno.Testing.Test] public static void test_732() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            int res = (int) new C ();
            if (res != 1)
                return 1;
            
            return 0;
        }
    }
}
