namespace Mono.test_18
{
    using Uno;
    
    class X {
        static int i;
        static int j;
        
        static void m ()
        {
            i = 0;
            j = 0;
            
            try {
                throw new ArgumentException ("Blah");
            } catch (ArgumentException){
                i = 1;
            } catch (Exception){
                i = 2;
            } finally {
                j = 1;
            }
        }
    
        static int ret (int a)
        {
            try {
                if (a == 1)
                    throw new Exception ();
                
                return 1;
            } catch {
                return 2;
            }
        }
        
        [Uno.Testing.Test] public static void test_18() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            m ();
            if (i != 1)
                return 1;
            if (j != 1)
                return 2;
    
            if (ret (1) != 2)
                return 3;
    
            if (ret (10) != 1)
                return 4;
            
            return 0;
        }
    }
}
