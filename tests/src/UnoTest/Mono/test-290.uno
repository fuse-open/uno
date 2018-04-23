namespace Mono.test_290
{
    // Distilled from report in http://lists.ximian.com/archives/public/mono-devel-list/2004-September/007777.html
    
    using Uno;
    
    class EntryPoint {
        delegate void EventHandler (object sender);
        static event EventHandler FooEvent;
        static void bar_f (object sender) {}
        [Uno.Testing.Test] public static void test_290() { Main(); }
        public static void Main() {
            if (FooEvent != null)
                FooEvent (null);
            object bar = new EventHandler (bar_f);
        }
    }
}
