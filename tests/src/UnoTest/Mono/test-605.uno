namespace Mono.test_605
{
    class TestA
    {
        public virtual string Method
        {
            get { return null; }
        }
    }
    
    class TestB : TestA
    {
        private string Method
        {
            get { return null; }
        }
    
        [Uno.Testing.Test] public static void test_605() { Main(); }
        public static void Main()
        {
        }
    }
}
