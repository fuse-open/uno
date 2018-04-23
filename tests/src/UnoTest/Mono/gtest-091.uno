namespace Mono.gtest_091
{
    using Uno;
    
    public class Foo<T>
    {
        Node node;
    
        public Node Test<V> ()
        {
            return node;
        }
    
        public class Node
        { }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_091() { Main(); }
        public static void Main()
        { }
    }
}
