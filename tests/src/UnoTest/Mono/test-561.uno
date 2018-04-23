namespace Mono.test_561
{
    abstract class A : I
    {
        protected abstract void M ();
            
        void I.M ()
        {
        }
    }
    
    interface I
    {
        void M ();
    }
    
    class C : A, I
    {
        protected override void M ()
        {
        }
        
        [Uno.Testing.Test] public static void test_561() { Main(); }
        public static void Main()
        {
        }
    }
}
