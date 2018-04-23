namespace Mono.test_657_p2
{
    #define DEBUG
    
    using Uno;
    
    namespace TestDebug
    {
        class C
        {
            public static void Method ()
            {
    #if !DEBUG
                throw new ApplicationException ("3");
    #endif
            }
        }
    }
}
