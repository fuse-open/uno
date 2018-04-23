namespace Mono.test_440
{
    public class A {
        public static implicit operator double (A a)
        {
            return 0.5;
        }
    
        // unlike CS0034 case, two or more implicit conversion on other 
        // than string is still valid.
        public static implicit operator int (A a)
        {
            return 0;
        }
    
        [Uno.Testing.Test] public static void test_440() { Main(); }
        public static void Main()
        {
            A a = new A ();
            object p = a + a;
        }
    }
}
