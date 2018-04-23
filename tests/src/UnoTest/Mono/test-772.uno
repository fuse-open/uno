namespace Mono.test_772
{
    using Uno;
    
    public class Test
    {
        [Uno.Testing.Test] public static void test_772() { Main(); }
        public static void Main()
        {
            Foo fu = new Foo (null);
        }
    }
    
    class Foo
    {
        public Foo (object o)
        {
            throw new ApplicationException ("wrong ctor");
        }
    
        public Foo (string s, params object[] args)
        {
        }
    }
}
