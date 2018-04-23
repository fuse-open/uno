namespace Mono.gtest_478
{
    using Uno;
    
    interface IA
    {
        void Foo ();
    }
    
    interface IG<T> : IA
    {
        void GenFoo ();
    }
    
    class M : IG<int>
    {
        public void Foo ()
        {
        }
        
        public void GenFoo ()
        {
        }
        
        [Uno.Testing.Test] public static void gtest_478() { Main(); }
        public static void Main()
        {
            IG<int> v = new M ();
            v.Foo ();
        }
    }
}
