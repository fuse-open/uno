namespace Mono.gtest_333
{
    using Uno;
    
    public static class Program
    {
        [Uno.Testing.Test] public static void gtest_333() { Main(); }
        public static void Main()
        {
            Exception ex1 = null ?? new Exception ();
            Exception ex2 = new Exception() ?? null;
        }
    }
}
