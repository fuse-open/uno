namespace Mono.test_754
{
    namespace Bug
    {
        public delegate void D ();
    
        public abstract class A
        {
            public abstract event D E;
        }
    
        public sealed class B : A
        {
            public override event D E
            {
                add { }
                remove { }
            }
        }
    
        class M
        {
            [Uno.Testing.Test] public static void test_754() { Main(); }
        public static void Main()
            {
            }
        }
    }
}
