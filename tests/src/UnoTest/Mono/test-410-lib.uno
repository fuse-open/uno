namespace Mono.test_410
{
    // Compiler options: -t:library

    namespace Q {
        public class A {
            public static new string ToString() {
                return "Hello world!";
            }
        }
    }
}
