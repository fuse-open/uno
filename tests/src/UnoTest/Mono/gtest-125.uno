namespace Mono.gtest_125
{
    using Uno;
    
    public interface IA<T> where T : struct {
    
    }
    
    public class B<T> : IA<T> where T:struct {
    
    }
    
    public class MainClass {
            [Uno.Testing.Test] public static void gtest_125() { Main(); }
        public static void Main() {}
    
    }
}
