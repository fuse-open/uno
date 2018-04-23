namespace Mono.test_744
{
    class M
    {
        sealed class Nested : C
        {
            protected override void Extra ()
            {
            }
        }
        
        [Uno.Testing.Test] public static void test_744() { Main(); }
        public static void Main()
        {
            new Nested ();
        }
    }
    
    abstract class A
    {
        protected abstract void AMethod ();
    }
    
    abstract class B : A
    {
        protected abstract void BMethod ();
    }
    
    abstract class C : B
    {
        protected override void AMethod ()
        {
        }
        
        protected override void BMethod ()
        {
        }
        
        protected abstract void Extra ();
    }
}
