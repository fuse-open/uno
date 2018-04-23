namespace Mono.gtest_569
{
    using Uno;
    
    class C
    {
        [Uno.Testing.Test] public static void gtest_569() { Main(); }
        public static void Main()
        {
            new TreeMap<int> ();
        }
    }
    
    public class TreeMap<T>
    {
        class Entry<U>
        {
            internal TreeMap<U>.Entry<int> field;
        }
    }
}
