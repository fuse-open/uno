namespace Mono.test_partial_28
{
    partial class A
    {
    }
    
    public class TestCase : A
    {    
        [Uno.Testing.Test] public static void test_partial_28() { Main(); }
        public static void Main()
        {
        }
    }
    
    public partial class A
    {
    }
}
