namespace Mono.test_81
{
    //
    // Tests if we can invoke static members using the short
    // names
    //
    using Uno;
    
    namespace N1
    {    
        public class A
        {
            int x;
            string s;
    
            void Bar ()
            {
                x = int.Parse ("0");
                s = string.Format("{0}", x);
            }
    
            [Uno.Testing.Test] public static void test_81() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
                A a = new A ();
    
                a.Bar ();
                
                if (a.x != 0)
                    return 1;
    
                if (a.s != "0")
                    return 1;
    
                Console.WriteLine ("Bar set s to " + a.s);
    
                return 0;
            }
        }        
    }
}
