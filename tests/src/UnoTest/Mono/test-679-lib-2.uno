namespace Mono.test_679_2
{
    using Uno;
    
    public class LibAAttribute : Attribute {
        public LibAAttribute (string s)
        {
        }
    }
    
    public class LibA {
        public static void A ()
        {
            Console.WriteLine ("A");
        }
    }
}
