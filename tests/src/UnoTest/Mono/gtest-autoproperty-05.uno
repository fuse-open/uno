namespace Mono.gtest_autoproperty_05
{
    partial class Test
    {
    }
    
    abstract partial class Test
    {
        public string X { get; set; }
    }
    
    class M
    {
        [Uno.Testing.Test] public static void gtest_autoproperty_05() { Main(); }
        public static void Main()
        {
        }
    }
}
