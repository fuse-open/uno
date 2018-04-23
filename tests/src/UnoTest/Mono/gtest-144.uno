namespace Mono.gtest_144
{
    using Uno;
    
    namespace Test
    {
        public class Application
        {
            [Uno.Testing.Test] public static void gtest_144() { Main(); }
        public static void Main()
            {
                string[] array = new string[10];
    
                Uno.Collections.IEnumerable<string> enumer = array;
            }
        }
    }
}
