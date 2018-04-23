namespace Mono.gtest_530
{
    class B : B2.IB
    {
        public interface IA
        {
        }
        
        [Uno.Testing.Test] public static void gtest_530() { Main(); }
        public static void Main()
        {
        }
    }
    
    class B2 : A
    {
        public interface IB
        {
        }
    }
    
    class A : G<int>
    {
    }
    
    class G<T>
    {
    }
}
