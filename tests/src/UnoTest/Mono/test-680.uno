namespace Mono.test_680
{
    using Uno;
    
    namespace AsStringProblem
    {
        class MainClass
        {
            [Uno.Testing.Test] public static void test_680() { Main(); }
        public static void Main()
            {
                object o = "Hello World";
                Console.WriteLine (o as string + "blah");
                Console.WriteLine (o is string + "blah");
                Console.WriteLine ((o as string) + "blah");
                Console.WriteLine ("blah" + o as string);
            }
        }
    }
}
