namespace Mono.gtest_082
{
    using Uno;
    
    public class Queue<T>
    {
        protected class Enumerator
        {
            Queue<T> queue;
    
            public Enumerator (Queue<T> queue)
            {
                this.queue = queue;
            }
        }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_082() { Main(); }
        public static void Main()
        { }
    }
}
