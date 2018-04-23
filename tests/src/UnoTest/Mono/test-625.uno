namespace Mono.test_625
{
    //
    // fixed
    //
    class Location {
        static public int Null {
            get {
                return 1;
            }
        }
    }
    
    class X {
        Location Location;
        X ()
        {
            int a = Location.Null;
        }
    
        [Uno.Testing.Test] public static void test_625() { Main(); }
        public static void Main() {}
    }
}
