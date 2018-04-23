namespace Mono.test_798
{
    using Uno;
    
    class A
    {
        public void Foo (out int a)
        {
            a = 100;
        }
    }
    
    class B : A
    {
        public void Foo (ref int a)
        {
            throw new ApplicationException ("should not be called");
        }
    }
    
    class C
    {
        [Uno.Testing.Test] public static void test_798() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            int x;
            new B().Foo (out x);
            if (x != 100)
                return 1;
            
            return 0;
        }
    }
}
