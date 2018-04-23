namespace Mono.gtest_594
{
    using Uno;
    
    public interface IFoo<U>
    {
        void Foo<T> () where T : C;
    }
    
    public class C : IA
    {
    }
    
    public interface IA
    {
    }
    
    class Y : IFoo<int>
    {
        public void Foo<T> () where T : C
        {
        }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_594() { Main(); }
        public static void Main()
        {
            new Y ();
        }
    }
}
