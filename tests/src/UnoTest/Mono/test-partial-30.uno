namespace Mono.test_partial_30
{
    // Partial parser tests, contextual sensitivity
    
    namespace PartialProblems
    {
        class Classes
        {
            class partial
            {
            }
            
            void M1 (partial formalParameter)
            {
            }
    
            partial M3 ()
            {
                return null;
            }
    
            partial field;
            
            [Uno.Testing.Test] public static void test_partial_30() { Main(); }
        public static void Main()
            {
            }
        }
    }
}
