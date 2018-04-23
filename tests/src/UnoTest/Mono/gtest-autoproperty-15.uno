namespace Mono.gtest_autoproperty_15
{
    public class C
    {
        public virtual int A { get; private set; }
    
        [Uno.Testing.Test] public static void gtest_autoproperty_15() { Main(); }
        public static void Main()
        {
        }
    }
}
