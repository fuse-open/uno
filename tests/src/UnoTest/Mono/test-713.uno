namespace Mono.test_713
{
    using Uno;
    
    class DispBar : IDisposable
    {
    
        public void Dispose ()
        {
            Console.WriteLine ("DispBar.Dispose");
        }
    }
    
    class Foo
    {
    
        public IDisposable GetBar ()
        {
            return new DispBar ();
        }
    }
    
    class Test
    {
    
        static Foo foo = new Foo ();
    
        public static bool TryThing ()
        {
            using (IDisposable disp = foo.GetBar ()) {
    
                bool bang = false;
                foo = null;
                return bang;
            }
        }
    
        [Uno.Testing.Test] public static void test_713() { Main(); }
        public static void Main()
        {
            TryThing ();
        }
    }
}
