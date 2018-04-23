namespace Mono.test_877
{
    using Uno;
    
    struct S
    {
        string value;
    
        public S (int arg)
        {
            throw new ApplicationException ();
        }
    }
    
    public class A
    {
        [Uno.Testing.Test] public static void test_877() { Main(); }
        public static void Main()
        {
        }
    }
}
