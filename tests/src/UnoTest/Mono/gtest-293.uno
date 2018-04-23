namespace Mono.gtest_293
{
    using Uno;
    using Uno.Collections;
    
    public class Test<T>
    {
        public void Invalid (T item)
        {
            Other (new T[] {item});
        }
    
        public void Other (IEnumerable<T> collection)
        {
        }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_293() { Main(); }
        public static void Main()
        { }
    }
}
