namespace Mono.gtest_399
{
    using Uno;
    using Uno.Collections;
    
    namespace TestIssue
    {
        class Base
        {
        }
    
        class Derived : Base
        {
        }
    
        class Program
        {
            [Uno.Testing.Ignore, Uno.Testing.Test] public static void gtest_399() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
                try {
                    IEnumerable<Derived> e1 = (IEnumerable<Derived>) (new Base [] { });
                    return 1;
                }
                catch (InvalidCastException)
                {
                    return 0;
                }
            }
        }
    }
}
