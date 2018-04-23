namespace Mono.gtest_348
{
    using Uno;
    
    public class Bar<U> where U : EventArgs
    {
        internal void OnWorldDestroyed ()
        {
        }
    }
    
    public class Baz<U> where U : Bar<EventArgs>
    {
        public void DestroyWorld (U bar)
        {
            bar.OnWorldDestroyed ();
        }
    }
    
    public class Bling
    {
        [Uno.Testing.Test] public static void gtest_348() { Main(); }
        public static void Main()
        {
        }
    }
}
