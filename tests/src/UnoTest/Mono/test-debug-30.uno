namespace Mono.test_debug_30
{
    class PragmaNewLinesParsing
    {
    #pragma warning disable RECS0029
    #pragma warning restore RECS0029
    
        void Foo ()
        {
            return;
        }
    
    #pragma warning disable RECS0029 // here
    #pragma warning restore RECS0029 /* end */
    
        [Uno.Testing.Test] public static void test_debug_30() { Main(); }
        public static void Main()
        {
            return;
        }
    }
}
