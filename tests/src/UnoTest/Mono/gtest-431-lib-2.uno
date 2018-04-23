namespace Mono.gtest_431_2
{
    // Compiler options: -t:library -noconfig -r:gtest-431-lib-1.dll
    
    using Uno;
    
    namespace Library {
    
        public class Foo {
        }
    
        public static class Extensions {
    
            public static void Bar (this Foo self)
            {
                Console.WriteLine ("Bar");
            }
        }
    }
}
