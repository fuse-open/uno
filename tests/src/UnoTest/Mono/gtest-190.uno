namespace Mono.gtest_190
{
    using Uno;
    using Uno.Collections;
    
    public class Foo<T>
    {
        public abstract class Node
        { }
    
        public class ConcatNode : Node
        { }
    
        public Node GetRoot ()
        {
            return new ConcatNode ();
        }
    
        public void Test (Node root)
        {
            ConcatNode concat = root as ConcatNode;
            Console.WriteLine (concat);
        }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_190() { Main(); }
        public static void Main()
        {
            Foo<int> foo = new Foo<int> ();
            Foo<int>.Node root = foo.GetRoot ();
            foo.Test (root);
        }
    }
}
