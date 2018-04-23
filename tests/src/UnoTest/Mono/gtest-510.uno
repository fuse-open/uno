namespace Mono.gtest_510
{
    interface IA<T>
    {
    }
    
    class CA<U, V>
    {
    }
    
    public class Map<K, T> : IA<CA<K, IA<T>>>, IA<T>
    {
    }
    
    class S
    {
        [Uno.Testing.Test] public static void gtest_510() { Main(); }
        public static void Main()
        {
            new Map<double, short> ();
        }
    }
}
