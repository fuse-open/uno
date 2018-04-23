namespace Mono.test_var_05
{
    // Tests variable type inference with the var keyword when using the "using" statement
    using Uno;
    
    public class MyClass : IDisposable
    {
        private string s;
        public MyClass (string s)
        {
            this.s = s;
        }
        public void Dispose()
        {
            s = "";
        }
    }
    
    public class Test
    {
        [Uno.Testing.Test] public static void test_var_05() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            using (var v = new MyClass("foo"))
                if (v.GetType() != typeof (MyClass))
                    return 1;
            
            return 0;
        }
    }
}
