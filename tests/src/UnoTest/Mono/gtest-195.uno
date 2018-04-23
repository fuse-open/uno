namespace Mono.gtest_195
{
    using Uno;
    using Uno.Collections;
    
    public class OrderedMultiDictionary<T,U>
    {
            private RedBlackTree<KeyValuePair<T,U>> tree;
    
            private void EnumerateKeys (RedBlackTree<KeyValuePair<T,U>>.RangeTester rangeTester)
            {
                    tree.EnumerateRange (rangeTester);
        }
    }
    
    internal class RedBlackTree<S>
    {
            public delegate int RangeTester (S item);
    
            public void EnumerateRange (RangeTester rangeTester)
        {
        }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_195() { Main(); }
        public static void Main()
        { }
    }
}
