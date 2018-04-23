namespace Mono.test_68
{
    //
    // Tests invocation of reference type functions with value type arguments
    //
    using Uno;
    enum A {
        Hello
    }
    
    class Y {
        public Y ()
        {
            value = 3;
        }
        public int value;
    }
    
    class X {
    
        [Uno.Testing.Ignore, Uno.Testing.Test] public static void test_68() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            if ("Hello" != A.Hello.ToString ())
                return 1;
    
            Console.WriteLine ("value is: " + (5.ToString ()));
            if (5.ToString () != "5")
                return 2;
    
            Y y = new Y ();
            if (y.value.ToString () != "3"){
                string x = y.value.ToString ();
                Console.WriteLine ("Got: {0} expected 3", x);
                return 3;        
            }
            Console.WriteLine ("Test ok");
            return 0;
        }
    }
}
