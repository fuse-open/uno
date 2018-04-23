namespace Mono.gtest_372
{
    public class TestClass<T> where T : class
    {
        public bool Check (T x, T y) { return x == y; }
    }
    
    public class C
    {
    }
    
    public class TestClass2<T> where T : C
    {
        public bool Check (T x, T y) { return x == y; }
    }
    
    public class X
    {
        [Uno.Testing.Test] public static void gtest_372() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            new TestClass<object> ().Check (null, null);
            new TestClass2<C> ().Check (null, null);
            return 0;
        }
    }
}
