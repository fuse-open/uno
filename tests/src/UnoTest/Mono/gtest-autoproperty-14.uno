namespace Mono.gtest_autoproperty_14
{
    public struct S
    {
        public int A { get; set;}
    
        public S (int a)
        {
            this.A = a;
        }
    
        [Uno.Testing.Test] public static void gtest_autoproperty_14() { Main(); }
        public static void Main()
        {
            
        }
    }
}
