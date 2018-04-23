namespace Mono.test_509
{
    public delegate void DelegateHandler();
    
    public interface EventInterface 
    {
        event DelegateHandler OnEvent;
    }
    
    public class BaseClass 
    {
        public event DelegateHandler OnEvent;
    }
    
    public class ExtendingClass : BaseClass, EventInterface 
    {
        [Uno.Testing.Test] public static void test_509() { Main(); }
        public static void Main()
        {
        }
    }
}
