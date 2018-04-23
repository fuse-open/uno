namespace Mono.gtest_342
{
    class Base<T> where T : Base<T>
    {
        public static implicit operator T (Base<T> t)
        {
            return (T) t;
        }
    }
    
    class TestMain {
        [Uno.Testing.Test] public static void gtest_342() { Main(new string[0]); }
        public static void Main(string[] args)
        {
        }
    }
}
