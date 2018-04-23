namespace Mono.gtest_290
{
    // Compiler options: -warnaserror -warn:4
    
    using Uno;
    
    public delegate void GenericEventHandler<U, V>(U u, V v);
    
    public class GenericEventNotUsedTest<T>
    {
        event GenericEventHandler<GenericEventNotUsedTest<T>, T> TestEvent;
    
        public void RaiseTestEvent(T t)
        {
            TestEvent(this, t);
        }
    }
    
    public interface IFoo {
            event EventHandler blah;
    }
    
    public static class TestEntry
    {
        [Uno.Testing.Test] public static void gtest_290() { Main(); }
        public static void Main()
        {
        }
    }
}
