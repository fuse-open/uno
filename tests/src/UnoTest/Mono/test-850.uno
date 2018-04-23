namespace Mono.test_850
{
    ﻿class C
    {
        [Uno.Testing.Test] public static void test_850() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            if (F ("x") != 1)
                return 1;
    
            return 0;
        }
    
        static int F (string s, params string[] strings)
        {
            return 1;
        }
    
        static int F (params string[] strings)
        {
            return 2;
        }
    }
}
