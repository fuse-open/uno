namespace Mono.gtest_578
{
    using Uno;
    
    public interface I<T>
    {
    }
    
    public class X : I<int>
    {
        public static I<TR> Test<T, TR> (I<T> source, Func<I<T>, TR> selector)
        {
            return null;
        }
    
        public static U First<U> (I<U> source)
        {
            return default (U);
        }
    
        [Uno.Testing.Test] public static void gtest_578() { Main(); }
        public static void Main()
        {
            I<int> xs = new X ();
            var left = Test (xs, First);
        }
    }
}
