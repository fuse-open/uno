namespace Mono.test_350
{
    using Uno;
    
    public class A
    {
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
        public class BAttribute : Attribute
        {
        }
    }
    
    
    [A.B()]
    public class C
    {
        [Uno.Testing.Test] public static void test_350() { Main(); }
        public static void Main() {}
    }
}
