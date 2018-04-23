namespace Mono.gtest_322
{
    public class MyBase<K, V>
    {
        public delegate void Callback (K key, V value);
        
        public MyBase (Callback insertionCallback)
        { }
    }
    
    public class X : MyBase<string, int>
    {
        public X (Callback cb)
            : base (cb)
        { }
    
        [Uno.Testing.Test] public static void gtest_322() { Main(); }
        public static void Main()
        { }
    }
}
