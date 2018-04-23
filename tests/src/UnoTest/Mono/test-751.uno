namespace Mono.test_751
{
    using SomeOtherNS;
    using LocalNS;
    using OneMoreNS;
    
    namespace SomeOtherNS.Compiler
    {
    }
    
    namespace OneMoreNS.Compiler
    {
    }
    
    namespace LocalNS
    {
        public class Compiler
        {
        }
    }
    
    namespace System.Local
    {
        class M
        {
            [Uno.Testing.Test] public static void test_751() { Main(); }
        public static void Main()
            {
                Compiler c = new LocalNS.Compiler ();
            }
        }
    }
}
