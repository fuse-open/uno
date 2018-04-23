namespace Mono.gtest_107
{
    using Uno;
    
    public delegate V Mapper<T,V> (T item);
    
    public interface ITree<T>
    {
        void Map<V> (Mapper<T,V> mapper);
    }
    
    public class Tree<T> : ITree<T>
    {
        T item;
    
        public Tree (T item)
        {
            this.item = item;
        }
    
        public void Map<V> (Mapper<T,V> mapper)
        {
            V new_item = mapper (item);
        }
    }
    
    class X
    {
        private string themap (int i)
        {
            return String.Format ("AA {0,4} BB", i);
        }
    
        void Test ()
        {
            Tree<int> tree = new Tree<int> (3);
            tree.Map (new Mapper<int,string> (themap));
        }
    
        [Uno.Testing.Test] public static void gtest_107() { Main(); }
        public static void Main()
        {
            X x = new X ();
            x.Test ();
        }
    }
}
