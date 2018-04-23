namespace Mono.test_448
{
    using Uno;
    
    public class MonoDivideProblem
    {
        static uint dividend = 0x80000000;
        static uint divisor = 1;
        [Uno.Testing.Test] public static void test_448() { Main(new string[0]); }
        public static void Main(string[] args)
        {
            Console.WriteLine("Dividend/Divisor = {0}", dividend/divisor);
        }
    
    }
}
