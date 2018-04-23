namespace Mono.gtest_604
{
    class A<T>
    {
        public interface IB { }
    }
    
    class E : A<int>.IB, A<string>.IB
    {
        [Uno.Testing.Test] public static void gtest_604() { Main(); }
        public static void Main()
        {
            new E ();
        }
    }
}
