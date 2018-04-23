namespace Mono.test_929
{
    public class Wibble 
    {
        [Uno.Testing.Test] public static void test_929() { Main(); }
        public static void Main() {
            Wibble w = new Wibble();
            if (w == (null)) {
            }
    
            if (w != (null)) {
            }
    
            if ((null) == w) {
            }
    
            if ((null) != w) {
            }
        }
    }
}
