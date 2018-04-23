namespace Mono.test_549
{
    //
    // from bug 80257
    // 
    
    public delegate object Get (Do d);
    
    
    
    public class Do {
            public void Register (Get g)
            {
            }
    
            public void Register (object g)
            {
            }
    
            static object MyGet (Do d)
            {
                    return null;
            }
    
            public void X ()
            {
                    Register (Do.MyGet);
            }
    }
    
    public class User {
            [Uno.Testing.Test] public static void test_549() { Main(); }
        public static void Main()
            {
            }
    }
}
