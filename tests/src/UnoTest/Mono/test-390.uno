namespace Mono.test_390
{
    class C
    {
        class O : M
        {
            public override void Foo ()
            {
            }    
        }
        
        class N
        {
            public virtual void Foo ()
            {
            }
        }
        
        class M : N
        {
        }
        
        [Uno.Testing.Test] public static void test_390() { Main(); }
        public static void Main()
        {
        }
    }
}
