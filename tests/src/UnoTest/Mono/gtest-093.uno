namespace Mono.gtest_093
{
    using Uno;
    
    public class Foo<T>
    {
        public readonly T Item;
    
        public Foo (T item)
        {
            this.Item = item;
        }
    
        static void maketreer (out Node rest)
        {
            rest = new Node ();
        }
    
        class Node
        { }
    
        public void Hello<U> ()
        {
            Foo<U>.Node node;
            Foo<U>.maketreer (out node);
        }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_093() { Main(); }
        public static void Main()
        { }
    }
}
