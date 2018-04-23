namespace Mono.test_partial_20
{
    using Uno;
    
    partial class A
    {
    }
    
    partial class A
    {
        public static int F = 3;
    }
    
    partial class B
    {
        public static int F = 4;    
    }
    
    partial class B
    {
    }
    
    
    public class C
    {
        [Uno.Testing.Test] public static void test_partial_20() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            if (A.F != 3)
                return 1;
            
            if (B.F != 4)
                return 2;
            
            Console.WriteLine ("OK");
            return 0;
        }
    }
}
