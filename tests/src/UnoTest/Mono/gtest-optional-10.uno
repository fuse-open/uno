namespace Mono.gtest_optional_10
{
    public class Program
    {    
        [Uno.Testing.Test] public static void gtest_optional_10() { Main(); }
        public static void Main()
        {
            new Program<object>();
        }
    }
    
    public class Program<T>
    {
        public Program(Generic<T> generic = null)
        {
        }
    }
    
    public class Generic<T>
    {    
    }
}
