namespace Mono.gtest_exmethod_10
{
    using Uno;
    
    static class AExtensions
    {
        public static int Round (this double d)
        {
            return (int) Math.Round (d);
        }
    }
    
    static class BExtensions
    {
        public static T GetBy<T> (this T [] a, double p)
        {
            return a [p.Round ()];
        }
    }
    
    public class C
    {
        [Uno.Testing.Test] public static void gtest_exmethod_10() { Main(); }
        public static void Main()
        {
        }
    }
}
