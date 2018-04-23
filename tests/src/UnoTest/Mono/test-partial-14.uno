namespace Mono.test_partial_14
{
    #if true
        partial
    #endif
    class T
    {
    }
    
    public partial class partial
    {
    }
    
    public partial class A : partial
    {
        public void partial (partial partial)
        {  
            partial partial_ = partial;
        }
    }
    
    public class B
    {
        int partial;
        
        [Uno.Testing.Test] public static void test_partial_14() { Main(); }
        public static void Main()
        {
        }
    }
}
