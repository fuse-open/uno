namespace Mono.gtest_146
{
    using Uno;
    
    public class MyLinkedList<T> {
        protected Node first;
    
        protected class Node
        {
            public T item;
    
            public Node (T item)
            {
                this.item = item; 
            }
        }
    }
    
    class SortedList<U> : MyLinkedList<U>
    {
        public void Insert (U x) { 
            Node node = first;
        }
    }
    
    class X {
        [Uno.Testing.Test] public static void gtest_146() { Main(); }
        public static void Main()
        { }
    }
}
