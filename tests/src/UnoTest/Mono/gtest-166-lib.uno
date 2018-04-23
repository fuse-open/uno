namespace Mono.gtest_166
{
    // Compiler options: -t:library
    
    using Uno;
    
    public class TestClass
    {
        public class B : A<Nested>
        {
        }
        
        public abstract class A<T>
        {
            public static Comparison<A<T>> Compare;
        }
        
        public class Nested
        {
        }
    }
}
