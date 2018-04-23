namespace Mono.test_521
{
    using Uno;
    
    public class Tests {
    
        public delegate void CallTargetWithContextN (object o, params object[] args);
    
        public static void CallWithContextN (object o, object[] args) {
        }
    
        [Uno.Testing.Test] public static void test_521() { Main(); }
        public static void Main() {
            object o = new CallTargetWithContextN (CallWithContextN);
        }
    }
}
