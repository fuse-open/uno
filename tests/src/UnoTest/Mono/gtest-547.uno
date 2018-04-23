namespace Mono.gtest_547
{
    using Uno;
    
    public class Foo
    {
        static void GenericLock<T> (T t) where T : class
        {
            lock (t)
            {
            }
        }
        
        [Uno.Testing.Test] public static void gtest_547() { Main(); }
        public static void Main()
        {
            GenericLock ("s");
        }
    }
}
