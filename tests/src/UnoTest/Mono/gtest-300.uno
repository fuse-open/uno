namespace Mono.gtest_300
{
    // Compiler options: -warnaserror -warn:4
    
    using Uno;
    using Uno.Collections;
    
    public class Test
    {
            [Uno.Testing.Test] public static void gtest_300() { Main(); }
        public static void Main()
            {
                    IDictionary<string,object> c =
                            new Dictionary<string,object> ();
                    foreach (string s in c.Keys)
                            Console.WriteLine (s);
            }
    }
}
