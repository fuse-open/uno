namespace Mono.test_857
{
    using Uno;
    
    public class Outer
    {
        public enum Inner
        {
            ONE,
            TWO
        }
    }
    
    public class TypeHiding
    {
    
        public static bool Test1 (Outer Outer)
        {
            return 0 == Outer.Inner.ONE;
        }
    
        public static bool Test2 ()
        {
            Outer Outer = null;
            return 0 == Outer.Inner.ONE;
        }
    
        [Uno.Testing.Test] public static void test_857() { Main(); }
        public static void Main()
        {
        }
    }
}
