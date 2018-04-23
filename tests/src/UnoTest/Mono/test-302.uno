namespace Mono.test_302
{
    using Uno;
    
    class A
    {
        class C : Exception { }
    
        public class B
        {
            class C : Exception { }
    
            public B () {
                try {
                    throw new A.B.C ();
                }
                catch (C e) {
                }
            }
        }
    
        [Uno.Testing.Test] public static void test_302() { Main(); }
        public static void Main()
        {
            object o = new A.B();
        }
    }
}
