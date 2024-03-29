namespace Mono.gtest_323
{
    public class MyBase<K, V>
    {
        public class Callback
        { }

        public void Hello (Callback cb)
        { }
    }

    public class X : MyBase<string, int>
    {
        public X (Callback cb)
        { }

        public void Test (Callback cb)
        {
            Hello (cb);
        }

        [Uno.Testing.Test] public static void gtest_323() { Main(); }
        public static void Main()
        { }
    }
}
