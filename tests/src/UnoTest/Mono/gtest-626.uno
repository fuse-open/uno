namespace Mono.gtest_626
{
    using Uno;
    
    class Program
    {
        public class Foo
        {
            public static bool MG (Foo t)
            {
                return false;
            }
        }
    
        public class Bar<T>
        {
            public static Bar<T> Create (Func<T, bool> a)
            {
                return null;
            }
    
            public static Bar<T> Create (Func<T, double> a, Func<T, bool> b = null)
            {
                throw new ApplicationException ();
            }
        }
    
        [Uno.Testing.Test] public static void gtest_626() { Main(); }
        public static void Main()
        {
            var x = Bar<Foo>.Create (Foo.MG);
        }
    }
}
