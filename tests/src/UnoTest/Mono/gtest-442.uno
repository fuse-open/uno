namespace Mono.gtest_442
{
    public abstract class NonGenericBase
    {
        public abstract int this[int i] { get; }
    }
    
    public abstract class GenericBase<T> : NonGenericBase
        where T : GenericBase<T>
    {
        T Instance { get { return default (T); } }
    
        public void Foo ()
        {
            int i = Instance[10];
        }
    }
    
    public class EntryPoint
    {
        [Uno.Testing.Test] public static void gtest_442() { Main(); }
        public static void Main() { }
    }
}
