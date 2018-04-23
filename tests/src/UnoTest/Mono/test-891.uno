namespace Mono.test_891
{
    interface I
    {
        int P { get; }
    }
    
    class B : I
    {
        int I.P { get { return 1; } }
    }
    
    class C : B
    {
        public int get_P ()
        {
            return 1;
        }
    
        [Uno.Testing.Test] public static void test_891() { Main(); }
        public static void Main()
        {
        }
    }
}
