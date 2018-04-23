namespace Mono.gtest_019
{
    // A very simple generic interface
    
    public interface IEnumerator<T> {
        T Current { get; } 
        bool MoveNext();
        void Reset();
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_019() { Main(); }
        public static void Main()
        { }
    }
}
