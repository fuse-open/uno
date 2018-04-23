namespace Mono.test_195
{
    using Uno;
    
    public class Y {
        static int count = 0;
        
        public static int C ()
        {
            count++;
            if (count == 2)
                throw new Exception ("error");
            return 1;
        }
    }
    
    class X {
        int a = Y.C ();
    
        X () : this (1)
        {
        }
    
        X (int a) {
        }
    
        [Uno.Testing.Ignore, Uno.Testing.Test] public static void test_195() { Main(); }
        public static void Main()
        {
            X x = new X ();
        }
    }
}
