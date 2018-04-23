namespace Mono.test_anon_94
{
    // Compiler options: -t:library
    
    using Uno;
    
    public class BaseClassLibrary
    {
        public int i;
        public virtual void Print (int arg) { Console.WriteLine ("BaseClass.Print"); i = arg; }
    }
}
