namespace Mono.test_794
{
    using Uno;
    
    internal interface IA
    {
        void SomeMethod ();
    }
    
    public class C : IA
    {
        [Uno.Testing.Test] public static void test_794() { Main(); }
        public static void Main()
        {
            new C ().TestCallOnly ();
        }
    
        // The body should contain call (not callvirt) only
        void TestCallOnly ()
        {
            int i = 0;
            var v = new int[0].GetType ();
    
            new C ().SomeMethod ();
            this.SomeMethod ();
            typeof (C).GetType ();
            new Action (SomeMethod).GetType ();
        }
    
        public void SomeMethod ()
        {
        }
    }
}
