namespace Mono.gtest_229
{
    using Uno;
    using Uno.Collections;
    using Uno.Collections;
    
    public class B : IComparable<B> {
        public int CompareTo (B b)
        {
            return 0;
        }
    }
    
    public class Tester
    {
        [Uno.Testing.Test] public static void gtest_229() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            B b = new B ();
    
            // This should be false
            if (b is IComparable<object>)
                return 1;
            return 0;
        }
    }
}
