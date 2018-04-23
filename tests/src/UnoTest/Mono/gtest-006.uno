namespace Mono.gtest_006
{
    // Using an array of a type parameter.
    
    class Stack<T>
    {
        int size;
        T[] data;
    
        public Stack ()
        {
            data = new T [200];
        }
    
        public void Push (T item)
        {
            data [size++] = item;
        }
    
        public T Pop ()
        {
            return data [--size];
        }
    
        public void Hello (T t)
        {
            Console.WriteLine ("Hello: {0}", t);
        }
    }
    
    class Test
    {
        [Uno.Testing.Test] public static void gtest_006() { Main(); }
        public static void Main()
        {
        }
    }
}
