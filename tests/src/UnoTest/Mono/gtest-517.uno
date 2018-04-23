namespace Mono.gtest_517
{
    public interface I : IA
    {
    }
    
    public interface IA
    {
    }
    
    public class C
    {
    }
    
    public class G<T> where T : C, I
    {
    }
    
    class Test<U> where U : C , I
    {
        G<U> field;
    }
    
    class M
    {
        [Uno.Testing.Test] public static void gtest_517() { Main(); }
        public static void Main()
        {
        }
    }
}
