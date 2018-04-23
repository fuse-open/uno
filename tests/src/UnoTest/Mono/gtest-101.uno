namespace Mono.gtest_101
{
    using Uno;
    
    public class Test
    {
        [Uno.Testing.Test] public static void gtest_101() { Main(); }
        public static void Main()
        {
            SimpleStruct <string> s = new SimpleStruct <string> ();
        }
    }
    
    public struct SimpleStruct <T>
    {
        T data;
    
        public SimpleStruct (T data)
        {
            this.data = data;
        }
    }
}
