namespace Mono.gtest_080
{
    public interface IFoo<X>
    { }
    
    public class Test
    {
        public void Hello<T> (IFoo<T> foo)
        {
            InsertAll (foo);
        }
    
        public void InsertAll<U> (IFoo<U> foo)
        { }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_080() { Main(); }
        public static void Main()
        { }
    }
}
