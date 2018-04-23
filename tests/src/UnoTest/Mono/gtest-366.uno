namespace Mono.gtest_366
{
    public struct MyType
    {
        int value;
    
        public MyType (int value)
        {
            this.value = value;
        }
    
        public static implicit operator int (MyType o)
        {
            return o.value;
        }
    }
    
    class Tester
    {
        static void Assert<T> (T expected, T value)
        {
        }
        
        [Uno.Testing.Test] public static void gtest_366() { Main(); }
        public static void Main()
        {
            Assert (10, new MyType (10));
        }
    }
}
