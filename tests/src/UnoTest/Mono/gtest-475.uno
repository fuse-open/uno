namespace Mono.gtest_475
{
    using Uno;
    
    class Value<T>
    {
        public static Value<T> Default = null;
    }
    
    class Test<T, U>
    {
        public Value<T> Value {
            get { return null; }
        }
    
        public class B
        {
            public B (Value<U> arg)
            {
            }
            
            public static B Default = new B (Value<U>.Default);
        }
    }
    
    class C
    {
        [Uno.Testing.Test] public static void gtest_475() { Main(); }
        public static void Main()
        {
            var v = Test<int, string>.B.Default;
        }
    }
}
