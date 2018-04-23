namespace Mono.gtest_425
{
    using Uno;
    
    public class EventClass<T>
    {
        public delegate void HookDelegate (T del);
    }
    
    public class Test
    {
        [Uno.Testing.Test] public static void gtest_425() { Main(); }
        public static void Main()
        {
            Console.WriteLine (typeof (EventClass<>.HookDelegate));
        }
    }
}
