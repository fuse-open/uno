namespace Mono.test_918
{
    public class Test
    {
        Test x;
    
        void Foo ()
        {
            {
                string x = "dd";
            }
    
            {
                x = null;
            }
    
            x = new Test ();
        }
    
        [Uno.Testing.Test] public static void test_918() { Main(); }
        public static void Main() { }
    }
}
