namespace Mono.gtest_518
{
    class Top<X>
    {
        public class C : I1<int>
        {
        }
    
        interface I1<T> : I2<T>
        {
        }
    
        interface I2<U>
        {
        }
    }
    
    class M
    {
        [Uno.Testing.Test] public static void gtest_518() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            return 0;
        }
    }
}
