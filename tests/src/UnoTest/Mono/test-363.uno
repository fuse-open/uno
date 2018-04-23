namespace Mono.test_363
{
    public class Location {
        public static readonly Location UnknownLocation = new 
    Location();
    
        private Location() {
        }
    }
    
    public abstract class Element {
        private Location _location = Location.UnknownLocation;
    
        protected virtual Location Location {
            get { return _location; }
            set { _location = value; }
        }
    }
    
    public class T {
        [Uno.Testing.Test] public static void test_363() { Main(); }
        public static void Main() { }
    }
}
