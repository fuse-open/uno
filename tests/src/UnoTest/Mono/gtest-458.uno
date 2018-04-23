namespace Mono.gtest_458
{
    public class MyList<T>
    {
        public class Helper<U, V> { }
    
        public Helper<U, V> GetHelper<U, V> ()
        {
            return null;
        }
    }
    
    class C
    {
        [Uno.Testing.Test] public static void gtest_458() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            new MyList<int> ().GetHelper<string, bool> ();
            return 0;
        }
    }
}
