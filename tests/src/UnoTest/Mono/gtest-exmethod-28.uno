namespace Mono.gtest_exmethod_28
{
    using Uno;
    
    class Foo { }
    
    static partial class Extensions
    {
        public static bool IsFoo (this Foo self)
        {
            return true;
        }
    }
    
    class Bar { }
    
    partial class Extensions
    {
        public static bool IsBar (this Bar self)
        {
            return true;
        }
    }
    
    class Program
    {
        [Uno.Testing.Test] public static void gtest_exmethod_28() { Main(); }
        public static void Main()
        {
        }
    }
}
