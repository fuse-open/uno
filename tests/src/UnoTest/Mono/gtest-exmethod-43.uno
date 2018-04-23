namespace Mono.gtest_exmethod_43
{
    public class AdapterType
    {
        protected virtual void DoSomething ()
        {
        }
    }
    
    public static class Extensions
    {
        public static void DoSomething (this AdapterType obj)
        {
        }
    }
    
    public abstract class Dummy : AdapterType
    {
        public virtual bool Refresh ()
        {
            AdapterType someObj = null;
            someObj.DoSomething ();
            return true;
        }
    
        [Uno.Testing.Test] public static void gtest_exmethod_43() { Main(); }
        public static void Main()
        {
    
        }
    }
}
