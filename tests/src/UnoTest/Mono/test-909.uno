namespace Mono.test_909
{
    using Uno;
    
    public struct S
    {
        public int A { get; private set;}
        public event EventHandler eh;
    
        public S (int a)
        {
            this.eh = null;
            A = a;
        }
    
        [Uno.Testing.Test] public static void test_909() { Main(); }
        public static void Main()
        {
        }
    }
}
