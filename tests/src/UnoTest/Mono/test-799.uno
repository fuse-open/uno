namespace Mono.test_799
{
    using Uno;
    
    public class Test2
    {
        protected internal class Foo
        {
        }
    
        private class Bar
        {
            public Bar (Test2.Foo baseArg4)
            {
            }
        }
        [Uno.Testing.Test] public static void test_799() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            new Bar (new Foo ());
            return 0;
        }
    }
}
