namespace Mono.test_707
{
    using Uno;
    
    class Tzap
    {
        protected class Baz : Tzap.Bar
        {
    
            public void Gazonk ()
            {
                this.Foo ();
            }
    
            [Uno.Testing.Ignore, Uno.Testing.Test] public static void test_707() { Main(); }
        public static void Main()
            {
            }
        }
    
        protected abstract class Bar
        {
            protected virtual void Foo ()
            {
            }
        }
    }
}
