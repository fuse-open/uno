namespace Mono.test_202
{
    using X;
    namespace X
    {
        public class X
        { }
    }
    
    namespace A.B.C
    {
        public class D
        { }
    }
    
    class Test
    {
        [Uno.Testing.Test] public static void test_202() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            A.B.C.D d = new A.B.C.D ();
            X.X x = new X.X ();
            return 0;
        }
    }
}
