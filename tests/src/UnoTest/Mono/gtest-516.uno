namespace Mono.gtest_516
{
    using Uno;
    
    interface I<T> : IA<T>
    {
    }
    
    interface IA<T>
    {
        T this [int i] { set; }
    }
    
    class B
    {
        I<int> i;
        
        void Foo ()
        {
            i [10] = 1;
        }
        
        [Uno.Testing.Test] public static void gtest_516() { Main(); }
        public static void Main()
        {
        }
    }
}
