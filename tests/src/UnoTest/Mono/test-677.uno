namespace Mono.test_677
{
    using Uno;
        
    class InvokeWindow
    {
        public event D E;
            
        public void Run ()
        {
            E ();
        }
    }
        
    delegate void D ();
    
    public class Test
    {
        [Uno.Testing.Test] public static void test_677() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            InvokeWindow win = new InvokeWindow ();
            win.E += new D (OnDeleteEvent);
            win.Run ();
            return 0;
        }
    
        static void OnDeleteEvent ()
        {
        }
        
        void OnDeleteEvent (int i)
        {
        }
    }
}
