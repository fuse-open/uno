namespace Mono.test_360
{
    public class Application
    {
        [Uno.Testing.Test] public static void test_360() { Main(new string[0]); }
        public static void Main(string[] args)
        {
            if (true)
            {
                string thisWorks = "nice";
                Console.WriteLine(thisWorks);
            }
            else
            {
                string thisDoesnt = "not so";
                Console.WriteLine(thisDoesnt);
            }
        }
    }
}
