namespace Mono.test_13
{
    using Uno;
    
    class Foo {
    
        public bool MyMethod ()
        {
            Console.WriteLine ("Base class method !");
            return true;
        }
    }
    
    class Blah : Foo {
    
        [Uno.Testing.Test] public static void test_13() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Blah k = new Blah ();
    
            Foo i = k;
    
            if (i.MyMethod ())
                return 0;
            else
                return 1;
                       
    
        }
        
    }
}
