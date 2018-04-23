namespace Mono.gtest_455
{
    using Uno;
    
    namespace ConsoleApplication1
    {
        class Program
        {
            [Uno.Testing.Test] public static void gtest_455() { Main(); }
        public static void Main()
            {
                object o = new object ();
                Inner<object>.Compare (o, o);
            }
        }
    
        public class Inner<T> where T : class
        {
            public static void Compare (object obj, T value)
            {
                if (obj != value) { }
            }
        }
    }
}
