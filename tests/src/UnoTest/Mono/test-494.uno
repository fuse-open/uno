namespace Mono.test_494
{
    using Uno.Text;
    
    namespace Agresso.Foundation {
        public class Function
        {
            [Uno.Testing.Test] public static void test_494() { Main(); }
        public static void Main() {}
        }
    
        public delegate void Translate(Function callback, 
            ref StringBuilder result);
        }
}
