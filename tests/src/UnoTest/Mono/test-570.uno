namespace Mono.test_570
{
    //Compiler options: -warnaserror -warn:4
    
    using Uno;
    interface IFoo
    {
    }
    
    class Bar
    {
    }
    
    class Program
    {
        [Uno.Testing.Test] public static void test_570() { Main(); }
        public static void Main()
        {
            IFoo foo = null;
            if (foo is IFoo)
                Console.WriteLine("got an IFoo"); // never prints
                
            Bar bar = null;
            if (bar is Bar)
                Console.WriteLine("got a bar"); // never prints
        }
    }
}
