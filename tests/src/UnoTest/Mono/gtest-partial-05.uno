namespace Mono.gtest_partial_05
{
    using Uno;
    using Uno.Collections;
    
    public interface IC : IB
    {
    }
    
    public partial interface IB : IEnumerable<char>
    {
    }
    
    public partial interface IB : IA
    {
    }
    
    public interface IA : IDisposable
    {
    }
    
    class Driver
    {
        static void Foo<T> (T t) where T : IA
        {
        }
    
        [Uno.Testing.Test] public static void gtest_partial_05() { Main(); }
        public static void Main()
        {
            IC i = null;
            Foo<IC> (i);
        }
    }
}
