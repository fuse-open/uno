namespace Mono.gtest_424
{
    using Uno;
    using Uno.Collections;
    
    public class A { }
    
    public class B : A { }
    
    public class Test
    {
        public static void Block (params A[] expressions)
        {
            throw new ApplicationException ();
        }
    
        public static void Block (IEnumerable<B> variables, params A[] expressions)
        {
        }
    
        [Uno.Testing.Test] public static void gtest_424() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            A e = new A ();
            Block (new B[] { }, e);
            return 0;
        }
    }
}
