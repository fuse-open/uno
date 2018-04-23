namespace Mono.test_714
{
    using Uno;
    
    class Hello : IFoo
    {
        void IBar.Test ()
        {
        }
        
        [Uno.Testing.Test] public static void test_714() { Main(); }
        public static void Main()
        {
        }
    }
    
    interface IBar
    {
        void Test ();
    }
    
    interface IFoo : IBar
    {
    }
}
