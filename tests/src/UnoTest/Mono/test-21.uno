namespace Mono.test_21
{
    using Uno;
    
    public class Blah {
    
        public class Foo {
    
            public Foo ()
            {
                Console.WriteLine ("Inside the Foo constructor now");
            }
            
            public int Bar (int i, int j)
            {
                Console.WriteLine ("The Bar method");
                return i+j;
            }
            
            
        }
    
        [Uno.Testing.Test] public static void test_21() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Foo f = new Foo ();
    
            int j = f.Bar (2, 3);
            Console.WriteLine ("Blah.Foo.Bar returned " + j);
            
            if (j == 5)
                return 0;
            else
                return 1;
    
        }
    
    }
}
