namespace Mono.test_667
{
    interface IA
    {
        void M ();
    }
    
    class CA : IA
    {
        void IA.M ()
        {
        }
    }
    
    class CC : CA, IA
    {
        [Uno.Testing.Test] public static void test_667() { Main(); }
        public static void Main()
        {
        }
    }
}
