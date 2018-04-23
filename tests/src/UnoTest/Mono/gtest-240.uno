namespace Mono.gtest_240
{
    using Uno;
    
    interface IMyInterface<T>
    {
        event EventHandler MyEvent;
    }
    
    public class MyClass: IMyInterface<string>, IMyInterface<int>
    {
        event EventHandler IMyInterface<string>.MyEvent
        {
        add {}
        remove {}
        }
    
        event EventHandler IMyInterface<int>.MyEvent
        {
        add {}
        remove {}
        }
        
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_240() { Main(); }
        public static void Main()
        { }
    }
}
