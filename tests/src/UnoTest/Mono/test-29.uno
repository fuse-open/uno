namespace Mono.test_29
{
    //
    // Versioning, should choose Derived.Add (1)
    //
    using Uno;
    
    class Base {
        public int val;
        
        public void Add (int x)
        {
            Console.WriteLine ("Incorrect method called");
     
            val = 1;
        }
    }
    
    class Derived : Base {
        public void Add (double x)
        {
            Console.WriteLine ("Calling the derived class with double! Excellent!");
            val = 2;
        }
    }
    
    class Demo {
    
        [Uno.Testing.Ignore, Uno.Testing.Test] public static void test_29() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Derived d = new Derived ();
    
            d.Add (1);
            if (d.val == 1)
                return 1;
    
            if (d.val == 2)
                return 0;
            return 2;
    
        }
    }
}
