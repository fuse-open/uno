namespace Mono.gtest_268
{
    public class Test
    {
            void Bar ()
            {
                    G<int> g = G<int>.Instance;
            }
    
            // When it goes outside, there is no error.
            public class G<T>
            {
                    public static G<T> Instance;
            }
    
        [Uno.Testing.Test] public static void gtest_268() { Main(); }
        public static void Main()
        { }
    }
}
