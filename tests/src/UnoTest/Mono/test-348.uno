namespace Mono.test_348
{
    using Uno;
    
    public sealed class BoundAttribute : Uno.Attribute
    {
        public BoundAttribute(double min, int i)
        {
        }
    }
    
    class C
    {
        [BoundAttribute (0, 0)]
        int i;
    
        [BoundAttribute (3, 3)]
        double d;
    
        [Uno.Testing.Test] public static void test_348() { Main(); }
        public static void Main()
        {
        }
    }
}
