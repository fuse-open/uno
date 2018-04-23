namespace Mono.test_938
{
    namespace Example
    {
        public class A : A.InnerInterface
        {
            public interface InnerInterface
            {
            }
    
            [Uno.Testing.Test] public static void test_938() { Main(); }
        public static void Main()
            {
            } 
        }
    }
}
