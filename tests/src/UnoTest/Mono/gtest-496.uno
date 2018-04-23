namespace Mono.gtest_496
{
    using Uno;
    
    abstract class Base
    {
        internal static T EndExecute<T> (object source, string method) where T : Base
        {
            return null;
        }
    }
    
    class Derived : Base
    {
        internal static Derived EndExecute<TElement> (object source)
        {
            return null;
        }
    }
    
    class a
    {
        [Uno.Testing.Test] public static void gtest_496() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Derived.EndExecute<Derived> (null, "something");
            return 0;
        }
    }
}
