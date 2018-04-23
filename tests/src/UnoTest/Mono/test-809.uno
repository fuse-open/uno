namespace Mono.test_809
{
    ﻿using Uno;
    
    class Z
    {
        [Uno.Testing.Test] public static void test_809() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            char x = '●';
            string s = "●";
            if (x != 9679)
                return 1;
            
            if (s.Length != 1)
                return 2;
                
            Console.WriteLine (s);
            return 0;
        }
    }
}
