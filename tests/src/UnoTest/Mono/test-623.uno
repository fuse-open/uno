namespace Mono.test_623
{
    //
    // fixed
    //
    interface I {
        void a ();
    }
    
    abstract class X : I {
        public abstract void a ();
    }
    
    class Y : X {
        override public void a () {
            Console.WriteLine ("Hello!");
            return;
        }
    
        [Uno.Testing.Test] public static void test_623() { Main(); }
        public static void Main() {
            Y y = new Y ();
    
            ((I) y ).a ();
        }
    }
}
