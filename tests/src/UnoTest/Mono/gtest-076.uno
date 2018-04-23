namespace Mono.gtest_076
{
    using Uno;
    
    struct Foo<T>
    {
        public T Data;
    
        public Foo (T data)
        {
            this.Data = data;
        }
    }
    
    class Test<T>
    {
        public Foo<T> GetFoo (T data)
        {
            return new Foo<T> (data);
        }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_076() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Test<long> test = new Test<long> ();
            Foo<long> foo = test.GetFoo (0x800);
            //
            // This is a very simple test, just make sure the struct
            // is returned correctly.  This was broken until recently
            // and I just fixed it on amd64.
            if (foo.Data != 0x800)
                return 1;
            return 0;
        }
    }
}
